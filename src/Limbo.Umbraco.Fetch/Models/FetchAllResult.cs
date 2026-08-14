using System.Text;
using Limbo.Umbraco.Fetch.Services;

namespace Limbo.Umbraco.Fetch.Models;

/// <summary>
/// Class representing the result of <see cref="FetchService.FetchAll"/>.
/// </summary>
public class FetchAllResult {

    /// <summary>
    /// Gets a string value representing the log of the fetch all operation.
    /// </summary>
    public string Log { get; }

    /// <summary>
    /// Initializes a new instance of the <c>FetchAllResult</c> class using the current content of the provided log buffer.
    /// </summary>
    /// <remarks>Copies <paramref name="log"/> by calling <c>ToString()</c>; later changes to the <see cref="StringBuilder"/> are not reflected.</remarks>
    /// <param name="log">The <see cref="StringBuilder"/> containing the log content to capture.</param>
    public FetchAllResult(StringBuilder log) {
        Log = log.ToString();
    }

}