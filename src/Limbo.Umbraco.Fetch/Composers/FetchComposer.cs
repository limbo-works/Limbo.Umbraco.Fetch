using System;
using System.Collections.Generic;
using Limbo.Umbraco.Fetch.Exceptions;
using Limbo.Umbraco.Fetch.Manifests;
using Limbo.Umbraco.Fetch.Models.Settings;
using Limbo.Umbraco.Fetch.Scheduling;
using Limbo.Umbraco.Fetch.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Configuration;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Fetch.Composers;

public class FetchComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {

        builder.Services.AddSingleton<FetchService>();
        builder.Services.AddOptions<FetchSettings>().Configure<IConfiguration, IWebHostEnvironment>(ConfigureBinder);
        builder.Services.AddSingleton<IPackageManifestReader, FetchManifestReader>();

        if (builder.Config.GetBoolean("Limbo:Fetch:Scheduling:Enabled", true)) {
            builder.Services.AddHostedService<FetchTask>();
        }

    }

    private static void ConfigureBinder(FetchSettings settings, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) {

        IConfigurationSection section = configuration.GetSection("Limbo:Fetch");

        IConfigurationSection feeds = section.GetSection("Feeds");

        HashSet<string> aliases = [];

        foreach (IConfigurationSection child in feeds.GetChildren()) {

            // Read from properties from their respective child sections
            string? alias = child.GetString("Alias");
            string? url = child.GetString("Url");
            string? path = child.GetString("Path");
            string? interval = child.GetString("Interval");

            // Validate required properties
            if (string.IsNullOrWhiteSpace(alias)) throw new FetchException("Feed does not specify an alias.");
            if (string.IsNullOrWhiteSpace(url)) throw new FetchException($"Feed with alias '{alias}' dot not specify a URL.");
            if (string.IsNullOrWhiteSpace(path)) throw new FetchException($"Feed with alias '{alias}' dot not specify a path.");
            if (string.IsNullOrWhiteSpace(interval)) throw new FetchException($"Feed with alias '{alias}' dot not specify an interval.");

            // Validate the alias
            if (!aliases.Add(alias)) throw new FetchException($"A feed already exists with alias '{alias}'...");

            TimeSpan intervalTimeSpan;
            try {
                intervalTimeSpan = FetchUtils.ParseTimeSpan(interval);
            } catch (Exception ex) {
                throw new FetchException($"Invalid interval specified for feed '{alias}': {interval}", ex);
            }

            // Initialize a new feed item
            FetchFeed feed = new() {
                Alias = alias,
                Url = url,
                Path = path,
                Interval = intervalTimeSpan
            };

            // Append the item to the list
            settings.Feeds.Add(feed);

        }

    }

}