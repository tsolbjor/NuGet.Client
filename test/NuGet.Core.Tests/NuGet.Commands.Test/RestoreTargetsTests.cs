using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.ProjectModel;
using NuGet.Test.Utility;
using Xunit;

namespace NuGet.Commands.Test
{
    public class RestoreTargetsTests
    {
        [Fact]
        public async Task RestoreTargets_RestoreWithNoRuntimes()
        {
            // Arrange
            var sources = new List<PackageSource>();

            var project1Json = @"
            {
              ""version"": ""1.0.0"",
              ""description"": """",
              ""authors"": [ ""author"" ],
              ""tags"": [ """" ],
              ""projectUrl"": """",
              ""licenseUrl"": """",
              ""frameworks"": {
                ""netstandard1.5"": {
                    ""dependencies"": {
                        ""packageA"": ""1.0.0""
                    }
                }
              }
            }";

            using (var workingDir = TestFileSystemUtility.CreateRandomTestFolder())
            {
                var packagesDir = new DirectoryInfo(Path.Combine(workingDir, "globalPackages"));
                var packageSource = new DirectoryInfo(Path.Combine(workingDir, "packageSource"));
                var project1 = new DirectoryInfo(Path.Combine(workingDir, "projects", "project1"));
                packagesDir.Create();
                packageSource.Create();
                project1.Create();
                sources.Add(new PackageSource(packageSource.FullName));

                File.WriteAllText(Path.Combine(project1.FullName, "project.json"), project1Json);

                var specPath1 = Path.Combine(project1.FullName, "project.json");
                var spec1 = JsonPackageSpecReader.GetPackageSpec(project1Json, "project1", specPath1);

                var logger = new TestLogger();
                var request = new RestoreRequest(spec1, sources, packagesDir.FullName, logger);

                request.LockFilePath = Path.Combine(project1.FullName, "project.lock.json");

                var packageA = new SimpleTestPackageContext()
                {
                    Id = "packageA"
                };
                packageA.AddFile("lib/netstandard1.5/a.dll");

                SimpleTestPackageUtility.CreatePackages(packageA, packageSource.FullName);

                // Act
                var command = new RestoreCommand(request);
                var result = await command.ExecuteAsync();
                var lockFile = result.LockFile;
                result.Commit(logger);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(1, lockFile.Libraries.Count);
            }
        }

        [Fact]
        public async Task RestoreTargets_RestoreWithRuntimes()
        {
            // Arrange
            var sources = new List<PackageSource>();

            var project1Json = @"
            {
              ""version"": ""1.0.0"",
              ""description"": """",
              ""authors"": [ ""author"" ],
              ""tags"": [ """" ],
              ""projectUrl"": """",
              ""licenseUrl"": """",
              ""frameworks"": {
                ""netstandard1.5"": {
                    ""dependencies"": {
                        ""packageA"": ""1.0.0""
                    }
                }
              }
            }";

            using (var workingDir = TestFileSystemUtility.CreateRandomTestFolder())
            {
                var packagesDir = new DirectoryInfo(Path.Combine(workingDir, "globalPackages"));
                var packageSource = new DirectoryInfo(Path.Combine(workingDir, "packageSource"));
                var project1 = new DirectoryInfo(Path.Combine(workingDir, "projects", "project1"));
                packagesDir.Create();
                packageSource.Create();
                project1.Create();
                sources.Add(new PackageSource(packageSource.FullName));

                File.WriteAllText(Path.Combine(project1.FullName, "project.json"), project1Json);

                var specPath1 = Path.Combine(project1.FullName, "project.json");
                var spec1 = JsonPackageSpecReader.GetPackageSpec(project1Json, "project1", specPath1);

                var logger = new TestLogger();
                var request = new RestoreRequest(spec1, sources, packagesDir.FullName, logger);

                request.LockFilePath = Path.Combine(project1.FullName, "project.lock.json");

                var packageA = new SimpleTestPackageContext()
                {
                    Id = "packageA"
                };

                packageA.AddFile("lib/netstandard1.5/a.dll");
                packageA.AddFile("native/a.dll");
                packageA.AddFile("runtimes/unix/native/a.dll");
                packageA.AddFile("runtimes/unix/lib/netstandard1.5/a.dll");
                packageA.AddFile("runtimes/win7/lib/netstandard1.5/a.dll");
                packageA.AddFile("runtimes/win7-x86/lib/netstandard1.5/a.dll");

                SimpleTestPackageUtility.CreatePackages(packageA, packageSource.FullName);

                // Act
                var command = new RestoreCommand(request);
                var result = await command.ExecuteAsync();
                var lockFile = result.LockFile;
                result.Commit(logger);

                var targetLib = lockFile.Targets.Single().Libraries.Single();

                // Assert
                Assert.True(result.Success);
                Assert.Equal(4, targetLib.RuntimeTargets.Count);
                Assert.Equal("runtimes/unix/lib/netstandard1.5/a.dll", targetLib.RuntimeTargets[0].Path);
                Assert.Equal("runtime", targetLib.RuntimeTargets[0].Properties["group"]);
                Assert.Equal("unix", targetLib.RuntimeTargets[0].Properties["rid"]);
            }
        }
    }
}
