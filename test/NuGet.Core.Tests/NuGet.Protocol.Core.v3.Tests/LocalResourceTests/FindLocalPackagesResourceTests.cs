using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Test.Utility;
using NuGet.Versioning;
using Xunit;

namespace NuGet.Protocol.Core.v3.Tests
{
    public class FindLocalPackagesResourceTests
    {
        [Fact]
        public async Task FindLocalPackagesResource_GetPackagesBasic()
        {
            using (var rootUnzip = TestFileSystemUtility.CreateRandomTestFolder())
            using (var rootV3 = TestFileSystemUtility.CreateRandomTestFolder())
            using (var rootV2 = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                var a = new PackageIdentity("a", NuGetVersion.Parse("1.0.0"));
                var a2 = new PackageIdentity("a", NuGetVersion.Parse("1.0.0-beta"));
                var b = new PackageIdentity("b", NuGetVersion.Parse("1.0.0"));
                var c = new PackageIdentity("c", NuGetVersion.Parse("1.0.0"));

                await CreateFeeds(rootV2, rootV3, rootUnzip, a, b, c, a2);

                var expected = new HashSet<PackageIdentity>()
                {
                    a,b,c,a2
                };

                var resources = new FindLocalPackagesResource[]
                {
                    new FindLocalPackagesResourceUnzipped(rootUnzip),
                    new FindLocalPackagesResourceV2(rootV2),
                    new FindLocalPackagesResourceV3(rootV3)
                };

                foreach (var resource in resources)
                {
                    // Act
                    var result = resource.GetPackages(testLogger, CancellationToken.None).ToList();

                    // Assert
                    Assert.True(expected.SetEquals(result.Select(p => p.Identity)));
                    Assert.True(expected.SetEquals(result.Select(p => p.Package.GetIdentity())));
                    Assert.True(expected.SetEquals(result.Select(p => p.Nuspec.GetIdentity())));
                    Assert.True(result.All(p => p.IsNupkg));
                }
            }
        }

        private async Task CreateFeeds(string rootV2, string rootV3, string rootUnzip, params PackageIdentity[] packages)
        {
            foreach (var package in packages)
            {
                SimpleTestPackageUtility.CreateFolderFeedV2(rootV2, package);
                await SimpleTestPackageUtility.CreateFolderFeedV3(rootV3, package);
                SimpleTestPackageUtility.CreateFolderFeedUnzip(rootUnzip, package);
            }
        }
    }
}
