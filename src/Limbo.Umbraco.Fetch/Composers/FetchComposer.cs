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
using Skybrud.Essentials.Time.Iso8601;
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
        ParseScheduling(section, settings);
        ParseFeeds(section, settings);
    }

    private static void ParseScheduling(IConfiguration section, FetchSettings settings) {

        // Get the section for scheduling settings
        IConfigurationSection scheduling = section.GetSection("Scheduling");

        // Read the "Enabled" property, defaulting to true if not specified. This is necessary
        // because the configuration binder does not support default values for boolean properties
        settings.Scheduling.IsEnabled = scheduling.GetBoolean("Enabled", true);

        // Read the "Delay" and "Interval" properties as strings
        string? delay = scheduling.GetString("Delay");
        string? interval = scheduling.GetString("Interval");

        // We don't need to parse normal TimeSpan values here, as the configuration binder will
        // handle that for us. We only need to parse integer values and ISO 8601 durations

        if (int.TryParse(delay, out int delayMinutes)) {
            settings.Scheduling.Delay = TimeSpan.FromMinutes(delayMinutes);
        } else if (Iso8601Utils.TryParseDuration(delay, out TimeSpan delayTimeSpan)) {
            settings.Scheduling.Delay = delayTimeSpan;
        }

        if (int.TryParse(interval, out int internalMinutes)) {
            settings.Scheduling.Interval = TimeSpan.FromMinutes(internalMinutes);
        } else if (Iso8601Utils.TryParseDuration(interval, out TimeSpan internalTimeSpan)) {
            settings.Scheduling.Interval = internalTimeSpan;
        }

    }

    private static void ParseFeeds(IConfiguration section, FetchSettings settings) {

        // Get the section for feed settings
        IConfigurationSection feeds = section.GetSection("Feeds");

        // Create a hash set to keep track of aliases and ensure uniqueness
        HashSet<string> aliases = [];

        // Iterate through each child section under "Feeds"
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