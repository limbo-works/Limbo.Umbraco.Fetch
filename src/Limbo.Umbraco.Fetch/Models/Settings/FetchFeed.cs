using System;
using Skybrud.Essentials.Http;
using Skybrud.Essentials.Time.Xml;

namespace Limbo.Umbraco.Fetch.Models.Settings {

    /// <summary>
    /// Class representing a feed (aka an URL to be downloaded).
    /// </summary>
    public class FetchFeed {

        #region Properties

        /// <summary>
        /// Gets or sets the alias of the feed.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the URL of the feed.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the path of the feed.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the path of the feed.
        /// </summary>
        public string AbsolutePath { get; set; }

        /// <summary>
        /// Gets or sets the internal.
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Gets or sets a callback to be used before getting the response.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Action<FetchFeed, IHttpRequest> PrepareRequest { get; set; }

        /// <summary>
        /// Gets or sets a callback invoked when the feed is successfully fetched.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Action<FetchFeed, IHttpRequest, IHttpResponse> OnSuccess { get; set; }

        /// <summary>
        /// Gets or sets a callback invoked when fetching the feed URL fails.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Action<FetchFeed, IHttpRequest, IHttpResponse, Exception> OnError { get; set; }

        internal string IntervalInternal {

            set {

                if (int.TryParse(value, out int minutes)) {
                    Interval = TimeSpan.FromMinutes(minutes);
                    return;
                }

                if (TimeSpan.TryParse(value, out TimeSpan ts)) {
                    Interval = ts;
                    return;
                }

                try {
                    Interval = XmlSchemaUtils.ParseDuration(value);
                } catch {
                    // ignore
                }

                throw new Exception($"Invalid interval specified: {value}");

            }

        }

        #endregion

    }

}