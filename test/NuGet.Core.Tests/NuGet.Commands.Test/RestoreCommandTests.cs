using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.ProjectModel;
using NuGet.Test.Utility;
using Xunit;

namespace NuGet.Commands.Test
{
    public class RestoreCommandTests
    {
        [Fact]
        public async Task RestoreCommand_PackageAndReferenceWithSameNameAndVersion()
        {
            // Arrange
            var sources = new List<PackageSource>();

            // Both TxMs reference packageA, but they are different types.
            // Verify that the reference does not show up under libraries.
            var project1Json = @"
            {
              ""version"": ""1.0.0"",
              ""description"": """",
              ""authors"": [ ""author"" ],
              ""tags"": [ """" ],
              ""projectUrl"": """",
              ""licenseUrl"": """",
              ""frameworks"": {
                ""net45"": {
                    ""frameworkAssemblies"": {
                         ""packageA"": ""4.0.0""
                    }
                },
                ""netstandard1.3"": {
                    ""dependencies"": {
                        ""packageA"": ""4.0.0""
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

                SimpleTestPackageUtility.CreateFullPackage(packageSource.FullName, "packageA", "4.0.0");

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
        public async Task RestoreCommand_RestoreProjectWithNoDependencies()
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
                ""net45"": {
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

                // Act
                var command = new RestoreCommand(request);
                var result = await command.ExecuteAsync();
                var lockFile = result.LockFile;
                result.Commit(logger);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(0, lockFile.Libraries.Count);
            }
        }

        [Fact]
        public async Task RestoreCommand_RestoresTools()
        {
            // Arrange
            var sources = new List<PackageSource>();
            var project1Json = @"
            {
              ""frameworks"": {
                ""net45"": { }
              },
              ""tools"": {
                ""packageB"": ""*""
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

                var packageA = new SimpleTestPackageContext("packageA");
                packageA.AddFile("lib/netstandard1.3/a.dll");

                var packageB = new SimpleTestPackageContext("packageB");
                packageB.AddFile("lib/netstandard1.4/b.dll");
                packageB.Dependencies.Add(packageA);

                SimpleTestPackageUtility.CreatePackages(packageSource.FullName, packageA, packageB);

                // Act
                var command = new RestoreCommand(request);
                var result = await command.ExecuteAsync();
                result.Commit(logger);

                // Assert
                Assert.True(
                    result.Success,
                    "The command did not succeed. Error messages: "
                    + Environment.NewLine + logger.ShowErrors());
                Assert.Equal(1, result.ToolRestoreResults.Count());

                var toolResult = result.ToolRestoreResults.First();
                Assert.NotNull(toolResult.LockFilePath);
                Assert.True(
                    File.Exists(toolResult.LockFilePath),
                    $"The tool lock file at {toolResult.LockFilePath} does not exist.");
                Assert.NotNull(toolResult.LockFile);
                Assert.Equal(1, toolResult.LockFile.Targets.Count);

                var target = toolResult.LockFile.Targets[0];
                Assert.Null(target.RuntimeIdentifier);
                Assert.Equal(FrameworkConstants.CommonFrameworks.NetStandardApp15, target.TargetFramework);
                Assert.Equal(2, target.Libraries.Count);

                var library = target.Libraries.First(l => l.Name == "packageB");
                Assert.NotNull(library);
                Assert.Equal("lib/netstandard1.4/b.dll", library.RuntimeAssemblies[0].Path);
            }
        }

        [Fact]
        public async Task RestoreCommand_FailsCommandWhenToolRestoreFails()
        {
            // Arrange
            var sources = new List<PackageSource>();
            var project1Json = @"
            {
              ""frameworks"": {
                ""net45"": { }
              },
              ""tools"": {
                ""packageA"": ""*""
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

                // The tool is not available on the source.

                // Act
                var command = new RestoreCommand(request);
                var result = await command.ExecuteAsync();
                result.Commit(logger);

                // Assert
                Assert.False(result.Success,
                    "The command should not have succeeded. Messages: "
                    + Environment.NewLine + logger.ShowMessages());
                Assert.Equal(1, result.ToolRestoreResults.Count());

                var toolResult = result.ToolRestoreResults.First();
                Assert.Null(toolResult.LockFilePath);
                Assert.NotNull(toolResult.LockFile);
                Assert.Equal(1, toolResult.LockFile.Targets.Count);

                var target = toolResult.LockFile.Targets[0];
                Assert.Null(target.RuntimeIdentifier);
                Assert.Equal(FrameworkConstants.CommonFrameworks.NetStandardApp15, target.TargetFramework);
                Assert.Equal(0, target.Libraries.Count);
            }
        }

        [Fact]
        public async Task RestoreCommand_FailsWithIncompatiblePackage()
        {
            // Arrange
            var sources = new List<PackageSource>();
            var project1Json = @"
            {
              ""frameworks"": {
                ""net45"": { }
              },
              ""tools"": {
                ""packageA"": ""*""
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

                var packageA = new SimpleTestPackageContext("packageA");
                packageA.AddFile("lib/net45/a.dll");

                SimpleTestPackageUtility.CreatePackages(packageSource.FullName, packageA);

                // Act
                var command = new RestoreCommand(request);
                var result = await command.ExecuteAsync();
                result.Commit(logger);

                // Assert
                Assert.False(
                    result.Success,
                    "The command should not have succeeded. Messages: "
                    + Environment.NewLine + logger.ShowMessages());
                Assert.Contains(
                    "packageA 1.0.0 is not compatible with .NETStandardApp,Version=v1.5.",
                    logger.ShowErrors());
                Assert.Equal(1, result.ToolRestoreResults.Count());

                var toolResult = result.ToolRestoreResults.First();
                Assert.NotNull(toolResult.LockFilePath);
                Assert.True(
                    File.Exists(toolResult.LockFilePath),
                    $"The tool lock file at {toolResult.LockFilePath} does not exist.");
                Assert.NotNull(toolResult.LockFile);
                Assert.Equal(1, toolResult.LockFile.Targets.Count);

                var target = toolResult.LockFile.Targets[0];
                Assert.Null(target.RuntimeIdentifier);
                Assert.Equal(FrameworkConstants.CommonFrameworks.NetStandardApp15, target.TargetFramework);
                Assert.Equal(1, target.Libraries.Count);

                var library = target.Libraries.First(l => l.Name == "packageA");
                Assert.NotNull(library);
                Assert.Equal(0, library.RuntimeAssemblies.Count);
            }
        }
    }
}
