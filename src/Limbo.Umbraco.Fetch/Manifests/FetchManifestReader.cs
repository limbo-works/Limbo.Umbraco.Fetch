using System.Collections.Generic;
using System.Threading.Tasks;
using Limbo.Umbraco.Fetch;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.TwentyThree.Manifests;

public class FetchManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        List<PackageManifest> temp = [
            new () {
                Id = FetchPackage.Alias,
                Name = FetchPackage.Name,
                AllowTelemetry = true,
                Version = FetchPackage.InformationalVersion,
                Extensions = []
            }
        ];

        return await Task.FromResult(temp);

    }

}