using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.Fetch;

/// <inheritdoc />
public class FetchManifestFilter : IManifestFilter {

    /// <inheritdoc />
    public void Filter(List<PackageManifest> manifests) {

        // Initialize a new manifest filter for this package
        PackageManifest manifest = new() {
            AllowPackageTelemetry = true,
            PackageId = FetchPackage.Alias,
            PackageName = FetchPackage.Name,
            Version = FetchPackage.InformationalVersion
        };

        // Append the manifest
        manifests.Add(manifest);

    }

}