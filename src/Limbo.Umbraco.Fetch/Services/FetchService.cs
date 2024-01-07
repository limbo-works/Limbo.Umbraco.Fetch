using System;
using System.IO;
using System.Text;
using Limbo.Umbraco.Fetch.Models.Settings;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Http;
using Skybrud.Essentials.Time;
using Skybrud.Essentials.Time.Iso8601;
using Umbraco.Cms.Core.Hosting;

namespace Limbo.Umbraco.Fetch.Services {

    /// <summary>
    /// Service for fetching configured feeds.
    /// </summary>
    public class FetchService {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IOptions<FetchSettings> _fetchSettings;

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified dependencies.
        /// </summary>
        /// <param name="hostingEnvironment">The current <see cref="IHostingEnvironment"/>.</param>
        /// <param name="fetchSettings">A reference to the fetch settings.</param>
        public FetchService(IHostingEnvironment hostingEnvironment, IOptions<FetchSettings> fetchSettings) {
            _hostingEnvironment = hostingEnvironment;
            _fetchSettings = fetchSettings;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Attempts to fetch all configured feeds.
        /// </summary>
        public StringBuilder FetchAll() {

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

                IHttpRequest request = null;
                IHttpResponse response = null;

                try {

                    if (string.IsNullOrWhiteSpace(feed.Url)) throw new PropertyNotSetException(nameof(feed.Url));
                    if (string.IsNullOrWhiteSpace(feed.Path)) throw new PropertyNotSetException(nameof(feed.Path));

                    if (string.IsNullOrWhiteSpace(feed.AbsolutePath)) {
                        feed.AbsolutePath = feed.Path.StartsWith("~/") ? _hostingEnvironment.MapPathContentRoot(feed.Path) : feed.Path;
                    }

                    string path1 = feed.AbsolutePath;
                    string path2 = $"{feed.AbsolutePath}.error";
                    string path3 = Path.GetDirectoryName(path1);

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

                    response = request.GetResponse();

                    log.AppendLine("> " + (int) response.StatusCode + " " + response.StatusCode);

                    if ((int) response.StatusCode >= 200 && (int) response.StatusCode < 300) {

                        File.WriteAllBytes(path1, response.BinaryBody);

                        feed.OnSuccess?.Invoke(feed, request, response);

                    } else {

                        File.WriteAllBytes(path2, response.BinaryBody);

                        feed.OnError?.Invoke(feed, request, response, null);

                    }

                } catch (Exception ex) {

                    log.AppendLine(ex + "");

                    feed?.OnError(feed, request, response, ex);

                }

            }

            return log;

        }

        #endregion

    }

}