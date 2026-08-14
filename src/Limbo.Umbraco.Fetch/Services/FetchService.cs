using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Limbo.Umbraco.Fetch.Models;
using Limbo.Umbraco.Fetch.Models.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Http;
using Skybrud.Essentials.Time;
using Skybrud.Essentials.Time.Iso8601;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Extensions;

namespace Limbo.Umbraco.Fetch.Services;

/// <summary>
/// Service for fetching configured feeds.
/// </summary>
/// <remarks>
/// Initializes a new instance based on the specified dependencies.
/// </remarks>
public class FetchService {

    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IOptions<FetchSettings> _fetchSettings;

    #region Constructors

    /// <summary>
    /// Service for fetching configured feeds.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance based on the specified dependencies.
    /// </remarks>
    /// <param name="webHostEnvironment">The current <see cref="IWebHostEnvironment"/>.</param>
    /// <param name="fetchSettings">A reference to the fetch settings.</param>
    public FetchService(IWebHostEnvironment webHostEnvironment, IOptions<FetchSettings> fetchSettings) {
        _webHostEnvironment = webHostEnvironment;
        _fetchSettings = fetchSettings;
    }

    #endregion

    #region Member methods

    /// <summary>
    /// Returns the absolute path of the data directory used by the <strong>Fetch</strong> package.
    /// </summary>
    /// <returns>The absolute path of the data directory.</returns>
    public virtual string GetDataDirectory() {
        return _webHostEnvironment.MapPathContentRoot($"{Constants.SystemDirectories.Data}/Limbo/Fetch");
    }

    /// <summary>
    /// Returns the absolute path of the specified <paramref name="feed"/>.
    /// </summary>
    /// <param name="feed">The feed to get the absolute path for.</param>
    /// <returns>The absolute path of the feed.</returns>
    /// <exception cref="PropertyNotSetException">If the <see cref="FetchFeed.Path"/> property is not set.</exception>
    public virtual string GetAbsolutePath(FetchFeed feed) {

        if (string.IsNullOrWhiteSpace(feed.Path)) throw new PropertyNotSetException(nameof(feed.Path));

        string path = feed.Path;

        // Replace {Alias} placeholder in path with the actual alias of the feed
        path = path.Replace("{Alias}", feed.Alias);

        // If the path is a file name, combine it with the data directory to get the absolute path
        if (IsFileName(path)) return Path.Combine(GetDataDirectory(), path);

        // If the path starts with "~/", map it to the content root of the web host environment
        if (path.StartsWith("~/")) return _webHostEnvironment.MapPathContentRoot(path);

        return path;

        static bool IsFileName(string value) {
            return !Path.IsPathRooted(value) && !value.Contains(Path.DirectorySeparatorChar) && !value.Contains(Path.AltDirectorySeparatorChar);
        }

    }

    /// <summary>
    /// Attempts to fetch all configured feeds.
    /// </summary>
    public async Task<FetchAllResult> FetchAll() {

        StringBuilder log = new();

        int i = 0;

        foreach (FetchFeed feed in _fetchSettings.Value.Feeds) {

            if (i++ > 0) {
                log.AppendLine();
                log.AppendLine();
                log.AppendLine();
            }

            log.AppendLine($"{EssentialsTime.UtcNow.ToString(Iso8601Constants.DateTimeMilliseconds)}");
            log.AppendLine($"Fetching feed with alias '{feed.Alias}'...");
            log.AppendLine();

            HttpRequest? request = null;
            IHttpResponse? response = null;

            try {

                if (string.IsNullOrWhiteSpace(feed.Url)) throw new PropertyNotSetException(nameof(feed.Url));
                if (string.IsNullOrWhiteSpace(feed.Path)) throw new PropertyNotSetException(nameof(feed.Path));

                string absolutePath = GetAbsolutePath(feed);

                string path1 = absolutePath;
                string path2 = $"{absolutePath}.error";
                string? path3 = Path.GetDirectoryName(path1);
                if (path3 == null) {
                    log.AppendLine($"> Unable to determine directory for path: {path1}");
                    continue;
                }
                if (!Directory.Exists(path3)) Directory.CreateDirectory(path3);

                if (File.GetLastWriteTimeUtc(path1) > DateTime.UtcNow.Subtract(feed.Interval)) {
                    log.AppendLine("> Skipping feed as the file was updated within the specified interval...");
                    continue;
                }

                if (File.GetLastWriteTimeUtc(path2) > DateTime.UtcNow.Subtract(feed.Interval)) {
                    log.AppendLine("> Skipping feed as the error file was updated within the specified interval...");
                    continue;
                }

                request = new HttpRequest {
                    Url = feed.Url
                };

                feed.PrepareRequest?.Invoke(feed, request);

                response = await request.GetResponseAsync();

                log.AppendLine("> " + (int) response.StatusCode + " " + response.StatusCode);

                if ((int) response.StatusCode >= 200 && (int) response.StatusCode < 300) {
                    await File.WriteAllBytesAsync(path1, response.BinaryBody);
                    feed.OnSuccess?.Invoke(feed, request, response);
                } else {
                    await File.WriteAllBytesAsync(path2, response.BinaryBody);
                    feed.OnError?.Invoke(feed, request, response, null);
                }

            } catch (Exception ex) {

                log.AppendLine(ex + "");

                feed.OnError?.Invoke(feed, request, response, ex);

            }

        }

        return new FetchAllResult(log);

    }

    #endregion

}