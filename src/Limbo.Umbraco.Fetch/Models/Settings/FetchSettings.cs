using System.Collections.Generic;

namespace Limbo.Umbraco.Fetch.Models.Settings;

/// <summary>
/// Class representing the <c>Limbo:Fetch</c> configuration section.
/// </summary>
public class FetchSettings {

    /// <summary>
    /// Gets a list of configured feeds.
    /// </summary>
    public List<FetchFeed> Feeds { get; internal set; } = new();

}