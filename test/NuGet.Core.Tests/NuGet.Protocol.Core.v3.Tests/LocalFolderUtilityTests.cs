using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Test.Utility;
using NuGet.Versioning;
using Xunit;

namespace NuGet.Protocol.Core.v3.Tests
{
    public class LocalFolderUtilityTests
    {
        [Fact]
        public void LocalFolderUtility_GetPackagesV2ValidPackage()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                SimpleTestPackageUtility.CreateFolderFeedV2(root, new PackageIdentity("a", NuGetVersion.Parse("1.0.0")));
                SimpleTestPackageUtility.CreateFolderFeedV2(root, new PackageIdentity("b", NuGetVersion.Parse("1.0.0")));
                SimpleTestPackageUtility.CreateFolderFeedV2(root, new PackageIdentity("c", NuGetVersion.Parse("1.0.0")));

                // Act
                var packages = LocalFolderUtility.GetPackagesV2(root, testLogger)
                    .OrderBy(p => p.Identity.Id, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(p => p.Identity.Version)
                    .ToList();

                // Assert
                Assert.Equal(3, packages.Count);
                Assert.Equal(new PackageIdentity("a", NuGetVersion.Parse("1.0.0")), packages[0].Identity);
                Assert.Equal(new PackageIdentity("b", NuGetVersion.Parse("1.0.0")), packages[1].Identity);
                Assert.Equal(new PackageIdentity("c", NuGetVersion.Parse("1.0.0")), packages[2].Identity);
            }
        }

        [Fact]
        public void LocalFolderUtility_GetPackagesV2ReadWithV3()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                SimpleTestPackageUtility.CreateFolderFeedV2(root, new PackageIdentity("a", NuGetVersion.Parse("1.0.0")));
                SimpleTestPackageUtility.CreateFolderFeedV2(root, new PackageIdentity("b", NuGetVersion.Parse("1.0.0")));
                SimpleTestPackageUtility.CreateFolderFeedV2(root, new PackageIdentity("c", NuGetVersion.Parse("1.0.0")));

                // Act
                var packages = LocalFolderUtility.GetPackagesV3(root, testLogger);

                // Assert
                Assert.Equal(0, packages.Count());
            }
        }

        [Fact]
        public async Task LocalFolderUtility_GetPackagesV3ValidPackage()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                await SimpleTestPackageUtility.CreateFolderFeedV3(root, new PackageIdentity("a", NuGetVersion.Parse("1.0.0")));
                await SimpleTestPackageUtility.CreateFolderFeedV3(root, new PackageIdentity("b", NuGetVersion.Parse("1.0.0")));
                await SimpleTestPackageUtility.CreateFolderFeedV3(root, new PackageIdentity("c", NuGetVersion.Parse("1.0.0")));

                // Act
                var packages = LocalFolderUtility.GetPackagesV3(root, testLogger)
                    .OrderBy(p => p.Identity.Id, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(p => p.Identity.Version)
                    .ToList();

                // Assert
                Assert.Equal(3, packages.Count);
                Assert.Equal(new PackageIdentity("a", NuGetVersion.Parse("1.0.0")), packages[0].Identity);
                Assert.Equal(new PackageIdentity("b", NuGetVersion.Parse("1.0.0")), packages[1].Identity);
                Assert.Equal(new PackageIdentity("c", NuGetVersion.Parse("1.0.0")), packages[2].Identity);
            }
        }

        [Fact]
        public async Task LocalFolderUtility_GetPackagesV3ReadWithV2()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                await SimpleTestPackageUtility.CreateFolderFeedV3(root, new PackageIdentity("a", NuGetVersion.Parse("1.0.0")));
                await SimpleTestPackageUtility.CreateFolderFeedV3(root, new PackageIdentity("b", NuGetVersion.Parse("1.0.0")));
                await SimpleTestPackageUtility.CreateFolderFeedV3(root, new PackageIdentity("c", NuGetVersion.Parse("1.0.0")));

                // Act
                var packages = LocalFolderUtility.GetPackagesV2(root, testLogger);

                // Assert
                Assert.Equal(0, packages.Count());
            }
        }

        [Fact]
        public void LocalFolderUtility_GetPackageV2()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                var a = new PackageIdentity("a", NuGetVersion.Parse("1.0.0"));
                var b = new PackageIdentity("b", NuGetVersion.Parse("1.0.0"));
                var c = new PackageIdentity("c", NuGetVersion.Parse("1.0.0"));

                SimpleTestPackageUtility.CreateFolderFeedV2(root, a);
                SimpleTestPackageUtility.CreateFolderFeedV2(root, b);
                SimpleTestPackageUtility.CreateFolderFeedV2(root, c);

                // Act
                var foundA = LocalFolderUtility.GetPackageV2(root, a, testLogger);

                // Assert
                Assert.Equal(a, foundA.Identity);
                Assert.Equal(a, foundA.Nuspec.GetIdentity());
                Assert.True(foundA.IsNupkg);
                Assert.Equal(a, foundA.Package.GetIdentity());
                Assert.Contains("a.1.0.0.nupkg", foundA.Path);
            }
        }

        [Fact]
        public void LocalFolderUtility_GetPackageV2NotFound()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                var a = new PackageIdentity("a", NuGetVersion.Parse("1.0.0"));
                var b = new PackageIdentity("b", NuGetVersion.Parse("1.0.0"));
                var c = new PackageIdentity("c", NuGetVersion.Parse("1.0.0"));

                SimpleTestPackageUtility.CreateFolderFeedV2(root, b);
                SimpleTestPackageUtility.CreateFolderFeedV2(root, c);

                // Act
                var foundA = LocalFolderUtility.GetPackageV2(root, a, testLogger);

                // Assert
                Assert.Null(foundA);
            }
        }

        [Fact]
        public void LocalFolderUtility_GetPackageV2NotFoundEmptyDir()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                var a = new PackageIdentity("a", NuGetVersion.Parse("1.0.0"));

                // Act
                var foundA = LocalFolderUtility.GetPackageV2(root, a, testLogger);

                // Assert
                Assert.Null(foundA);
            }
        }

        [Fact]
        public void LocalFolderUtility_GetPackageV2NotFoundMissingDir()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                var a = new PackageIdentity("a", NuGetVersion.Parse("1.0.0"));

                // Act
                var foundA = LocalFolderUtility.GetPackageV2(Path.Combine(root, "missing"), a, testLogger);

                // Assert
                Assert.Null(foundA);
            }
        }

        [Fact]
        public void LocalFolderUtility_GetPackagesV2NotFoundEmptyDir()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();

                // Act
                var packages = LocalFolderUtility.GetPackagesV2(root, testLogger).ToList();

                // Assert
                Assert.Equal(0, packages.Count);
            }
        }

        [Fact]
        public void LocalFolderUtility_GetPackagesV2NotFoundMissingDir()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();

                // Act
                var packages = LocalFolderUtility.GetPackagesV2(Path.Combine(root, "missing"), testLogger).ToList();

                // Assert
                Assert.Equal(0, packages.Count);
            }
        }

        [Fact]
        public void LocalFolderUtility_GetPackagesByIdV2NotFoundEmptyDir()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();

                // Act
                var packages = LocalFolderUtility.GetPackagesV2(root, "a", testLogger).ToList();

                // Assert
                Assert.Equal(0, packages.Count);
            }
        }

        [Fact]
        public void LocalFolderUtility_GetPackagesByIdV2NotFoundMissingDir()
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();

                // Act
                var packages = LocalFolderUtility.GetPackagesV2(root, "a", testLogger).ToList();

                // Assert
                Assert.Equal(0, packages.Count);
            }
        }

        [Theory]
        [InlineData("0.0.0")]
        [InlineData("0.0.1")]
        [InlineData("1.0.0-BETA")]
        [InlineData("1.0.0")]
        [InlineData("1.0")]
        [InlineData("1.0.0.0")]
        [InlineData("1.0.1")]
        [InlineData("1.0.01")]
        [InlineData("00000001.000000000.0000000001")]
        [InlineData("1.0.01-alpha")]
        [InlineData("1.0.1-alpha.1.2.3")]
        [InlineData("1.0.1-alpha.1.2.3+metadata")]
        [InlineData("1.0.1-alpha.1.2.3+a.b.c.d")]
        [InlineData("1.0.1-alpha.10.a")]
        public void LocalFolderUtility_VerifyPackageCanBeFoundV2_NonNormalizedOnDisk(string versionString)
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                var version = NuGetVersion.Parse(versionString);
                var normalizedVersion = NuGetVersion.Parse(NuGetVersion.Parse(versionString).ToNormalizedString());
                var identity = new PackageIdentity("a", version);

                SimpleTestPackageUtility.CreateFolderFeedV2(root, identity);

                // Act
                var findPackage = LocalFolderUtility.GetPackageV2(root, "a", version, testLogger);
                var findPackageNormalized = LocalFolderUtility.GetPackageV2(root, "a", normalizedVersion, testLogger);
                var findById = LocalFolderUtility.GetPackagesV2(root, "a", testLogger).Single();
                var findAll = LocalFolderUtility.GetPackagesV2(root, testLogger).Single();

                // Assert
                Assert.Equal(identity, findPackage.Identity);
                Assert.Equal(identity, findPackageNormalized.Identity);
                Assert.Equal(identity, findById.Identity);
                Assert.Equal(identity, findAll.Identity);
            }
        }

        [Theory]
        [InlineData("0.0.0")]
        [InlineData("0.0.1")]
        [InlineData("1.0.0-BETA")]
        [InlineData("1.0.0")]
        [InlineData("1.0")]
        [InlineData("1.0.0.0")]
        [InlineData("1.0.1")]
        [InlineData("1.0.01")]
        [InlineData("00000001.000000000.0000000001")]
        [InlineData("1.0.01-alpha")]
        [InlineData("1.0.1-alpha.1.2.3")]
        [InlineData("1.0.1-alpha.1.2.3+metadata")]
        [InlineData("1.0.1-alpha.1.2.3+a.b.c.d")]
        [InlineData("1.0.1-alpha.10.a")]
        public void LocalFolderUtility_VerifyPackageCanBeFoundV2_NormalizedOnDisk(string versionString)
        {
            using (var root = TestFileSystemUtility.CreateRandomTestFolder())
            {
                // Arrange
                var testLogger = new TestLogger();
                var version = NuGetVersion.Parse(versionString);
                var normalizedVersion = NuGetVersion.Parse(NuGetVersion.Parse(versionString).ToNormalizedString());
                var identity = new PackageIdentity("a", version);
                var normalizedIdentity = new PackageIdentity("a", normalizedVersion);

                SimpleTestPackageUtility.CreateFolderFeedV2(root, normalizedIdentity);

                // Act
                var findPackage = LocalFolderUtility.GetPackageV2(root, "a", version, testLogger);
                var findPackageNormalized = LocalFolderUtility.GetPackageV2(root, "a", normalizedVersion, testLogger);
                var findById = LocalFolderUtility.GetPackagesV2(root, "a", testLogger).Single();
                var findAll = LocalFolderUtility.GetPackagesV2(root, testLogger).Single();

                // Assert
                Assert.Equal(identity, findPackage.Identity);
                Assert.Equal(identity, findPackageNormalized.Identity);
                Assert.Equal(identity, findById.Identity);
                Assert.Equal(identity, findAll.Identity);
            }
        }
    }
}
