using System;
using System.Collections.Generic;
using Limbo.Umbraco.Fetch.Models.Settings;
using Limbo.Umbraco.Fetch.Scheduling;
using Limbo.Umbraco.Fetch.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Time.Xml;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Extensions;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Fetch;

public class FetchComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {
        builder.Services.AddSingleton<FetchService>();
        builder.Services.AddOptions<FetchSettings>().Configure<IConfiguration, IWebHostEnvironment>(ConfigureBinder);
        builder.Services.AddHostedService<FetchTask>();
    }

    private static void ConfigureBinder(FetchSettings settings, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) {

        IConfigurationSection section = configuration.GetSection("Limbo:Fetch");

        IConfigurationSection feeds = section?.GetSection("Feeds");
        if (feeds == null) return;

        HashSet<string> aliases = new();

        foreach (IConfigurationSection child in feeds.GetChildren()) {

            // Read from properties from their respective child sections
            string alias = child.GetSection("Alias")?.Value;
            string url = child.GetSection("Url")?.Value;
            string path = child.GetSection("Path")?.Value;
            string interval = child.GetSection("Interval")?.Value;

            // Validate required properties
            if (string.IsNullOrWhiteSpace(alias)) throw new Exception("Feed does not specify an alias.");
            if (string.IsNullOrWhiteSpace(url)) throw new Exception($"Feed with alias '{alias}' dot not specify a URL.");
            if (string.IsNullOrWhiteSpace(path)) throw new Exception($"Feed with alias '{alias}' dot not specify a path.");

            // Validate the alias
            if (aliases.Contains(alias)) throw new Exception($"A feed already exists with alias '{alias}'...");
            aliases.Add(alias);

            // Initialize a new feed item
            FetchFeed feed = new() {
                Alias = alias,
                Url = url,
                Path = path,
                AbsolutePath = path.StartsWith("~/") ? webHostEnvironment.MapPathContentRoot(path) : path
            };

            // Parse the interval
            feed.Interval = ParseTimeSpan(feed, interval);

            // Append the item to the list
            settings.Feeds.Add(feed);

        }

    }

    private static TimeSpan ParseTimeSpan(FetchFeed feed, string value) {

        // Set interval to 24 hours if not explicitly specified
        if (string.IsNullOrWhiteSpace(value)) return TimeSpan.FromHours(24);

        // If configured value is a numeric value, we assume it's in minutes
        if (int.TryParse(value, out int minutes)) {
            return TimeSpan.FromMinutes(minutes);
        }

        // Try to parse as TimeSpan
        if (TimeSpan.TryParse(value, out TimeSpan timeSpan)) {
            return timeSpan;
        }

        // Try to parse the XML schema duration (also ISO 8601 duration)
        try {
            return XmlSchemaUtils.ParseDuration(value);
        } catch {
            // ignore
        }

        throw new Exception($"Invalid interval specified for feed '{feed.Alias}': {value}");

    }

}