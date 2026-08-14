using System;
using Limbo.Umbraco.Fetch.Scheduling;
using Microsoft.Extensions.Configuration;

namespace Limbo.Umbraco.Fetch.Models.Settings;

/// <summary>
/// Class exposing settings for <see cref="FetchTask"/>.
/// </summary>
public class FetchSchedulingSettings {

    /// <summary>
    /// Gets or sets whether scheduling is enabled. Default is <see langword="true"/>.
    /// </summary>
    [ConfigurationKeyName("Enabled")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the initial delay from startup and until the job is run the first time. Default <strong>1 minute</strong>.
    /// </summary>
    public TimeSpan Delay { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the interval between each run. Default is <strong>1 minute</strong>.
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);

}