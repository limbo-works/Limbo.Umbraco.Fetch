using System.Collections.Generic;
using System.Linq;

namespace Limbo.Umbraco.Fetch.Models.Settings;

/// <summary>
/// Class representing the <c>Limbo:Fetch</c> configuration section.
/// </summary>
public class FetchSettings {

    /// <summary>
    /// Gets or sets the scheduling settings for fetch operations.
    /// </summary>
    public FetchSchedulingSettings Scheduling { get; set; } = new();

    /// <summary>
    /// Gets a list of configured feeds.
    /// </summary>
    public List<FetchFeed> Feeds { get; set; } = [];

    /// <summary>
    /// Returns whether a feed with the specified <paramref name="alias"/> exists in the <see cref="Feeds"/> collection.
    /// </summary>
    /// <param name="alias">The alias of the feed to check for.</param>
    /// <returns><see langword="true"/> if a feed with the specified alias exists; otherwise, <see langword="false"/>.</returns>
    public bool HasFeed(string alias) {
        return Feeds.FirstOrDefault(x => x.Alias == alias) is not null;
    }

}