using System;
using Limbo.Umbraco.Fetch.Exceptions;
using Limbo.Umbraco.Fetch.Models.Settings;

namespace Limbo.Umbraco.Fetch.Extensions;

/// <summary>
/// Static class with various extension methods for the <strong>Fetch</strong> package.
/// </summary>
public static class FetchExtensions {

    /// <summary>
    /// Adds a new feed definition to the specified fetch settings.
    /// </summary>
    /// <typeparam name="TSettings">The type of fetch settings to update.</typeparam>
    /// <param name="settings">The settings instance to update.</param>
    /// <param name="alias">The unique alias for the feed.</param>
    /// <param name="url">The source URL of the feed.</param>
    /// <param name="path">The destination path used for the feed.</param>
    /// <param name="interval">The polling interval for the feed.</param>
    /// <returns>The updated settings instance containing the added feed.</returns>
    /// <exception cref="FetchException">Thrown when a feed with <paramref name="alias" /> has already been added.</exception>
    public static TSettings AddFeed<TSettings>(TSettings settings, string alias, string url, string path, TimeSpan interval) where TSettings : FetchSettings {

        if (settings.HasFeed(alias)) throw new FetchException($"A feed already exists with alias '{alias}'...");

        FetchFeed feed = new() {
            Alias = alias,
            Url = url,
            Path = path,
            Interval = interval
        };

        settings.Feeds.Add(feed);

        return settings;

    }

    /// <summary>
    /// Adds a feed using an interval specified in minutes.
    /// </summary>
    /// <typeparam name="TSettings">The settings type.</typeparam>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The feed alias.</param>
    /// <param name="url">The feed URL.</param>
    /// <param name="path">The feed path.</param>
    /// <param name="interval">The interval in minutes.</param>
    /// <returns>The updated settings instance.</returns>
    /// <exception cref="FetchException">Thrown when a feed with <paramref name="alias" /> has already been added.</exception>
    public static TSettings AddFeed<TSettings>(TSettings settings, string alias, string url, string path, int interval) where TSettings : FetchSettings {
        return AddFeed(settings, alias, url, path, TimeSpan.FromMinutes(interval));
    }

    /// <summary>
    /// Adds a feed using an interval specified as a string.
    /// </summary>
    /// <typeparam name="TSettings">The settings type.</typeparam>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The feed alias.</param>
    /// <param name="url">The feed URL.</param>
    /// <param name="path">The feed path.</param>
    /// <param name="interval">The interval value.</param>
    /// <returns>The updated settings instance.</returns>
    /// <remarks>
    /// Valid interval formats include:
    /// <list type="bullet">
    /// <item><description>Numeric minutes: <c>5</c>, <c>30</c>, <c>120</c></description></item>
    /// <item><description><see cref="TimeSpan" /> formats: <c>00:05:00</c>, <c>01:30:00</c>, <c>2.00:00:00</c></description></item>
    /// <item><description>XML Schema / ISO 8601 durations: <c>PT5M</c>, <c>PT30M</c>, <c>P1DT2H</c></description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="FetchException">Thrown when a feed with <paramref name="alias" /> has already been added or <paramref name="interval" /> is not a valid interval.</exception>
    public static TSettings AddFeed<TSettings>(TSettings settings, string alias, string url, string path, string interval) where TSettings : FetchSettings {
        return AddFeed(settings, alias, url, path, FetchUtils.ParseTimeSpan(interval));
    }

}