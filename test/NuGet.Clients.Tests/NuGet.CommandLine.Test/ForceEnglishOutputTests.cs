using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Test.Utility;
using Xunit;
using Console = NuGet.Common.Console;

namespace NuGet.CommandLine.Test
{
    public class ForceEnglishOutputTests
    {
        [Fact]
        public void ForceEnglishOutput_DetectsAllResources()
        {
            // Arrange
            var nugetExe = Util.GetNuGetExePath();
            var args = new[] { "help", "-forceEnglishOutput", "-verbosity", "detailed" };
            var listStart = "Localization was disabled on the following types:";

            // NOTE: This list must be modified as more resources are added to the codebase.
            var expected = new HashSet<string>(new[]
            {
                "NuGet.CommandLine.NuGetCommand",
                "NuGet.CommandLine.NuGetResources",
                "NuGet.Commands.Rules.AnalysisResources",
                "NuGet.Commands.Strings",
                "NuGet.Common.Strings",
                "NuGet.CommonResources",
                "NuGet.Configuration.Resources",
                "NuGet.Credentials.Resources",
                "NuGet.Frameworks.Strings",
                "NuGet.LocalizedResourceManager",
                "NuGet.PackageManagement.Strings",
                "NuGet.Packaging.Core.Strings",
                "NuGet.Packaging.PackageCreation.Resources.NuGetResources",
                "NuGet.Packaging.Strings",
                "NuGet.ProjectManagement.Strings",
                "NuGet.ProjectModel.Strings",
                "NuGet.Protocol.Core.v2.Strings",
                "NuGet.Protocol.Core.v3.Strings",
                "NuGet.Protocol.VisualStudio.Strings",
                "NuGet.Resolver.Strings",
                "NuGet.Resources.AnalysisResources",
                "NuGet.Resources.NuGetResources",
                "NuGet.Versioning.Resources"
            });

            // A couple work-arounds are necessary for differences between the test environment and a user really
            // using the product.

            // 1) Exclude test resources.
            var ignored = new HashSet<string>(new[]
            {
                "NuGet.CommandLine.Test.MockServerResource"
            });

            // 2) Force NuGet.Credentials to be placed in the test directory.
            new ConsoleCredentialProvider(new Console());

            // Act
            var result = CommandRunner.Run(
                nugetExe,
                Directory.GetCurrentDirectory(),
                string.Join(" ", args),
                waitForExit: true);

            // Assert
            Assert.True(
                result.Item1 == 0,
                "The command failed. STDOUT:" +
                Environment.NewLine +
                result.Item2 +
                Environment.NewLine +
                "STDERR:" +
                Environment.NewLine +
                result.Item3); 
            Assert.Contains(listStart, result.Item2);

            // Collect the list of types that actually had their localization disabled.
            var index = result.Item2.LastIndexOf(listStart);
            var actual = new HashSet<string>();
            using (var reader = new StringReader(result.Item2))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(" - "))
                    {
                        actual.Add(line.Trim(new[] { ' ', '-' }));
                    }
                }
            }

            // Remove ignored resources.
            actual.ExceptWith(ignored);

            // Find types that should have been localized but were not.
            var missing = expected.Except(actual);

            // Find types that were localized but should not have been.
            var extra = actual.Except(expected);

            Assert.False(
                missing.Any() || extra.Any(),
                $"The following {missing.Count()} resource(s) should have been localized:" +
                Environment.NewLine +
                string.Join(Environment.NewLine, missing) +
                Environment.NewLine +
                $"The following {extra.Count()} resource(s) should not have been localized:" +
                Environment.NewLine +
                string.Join(Environment.NewLine, extra));
        }
    }
}
