using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;

namespace NuGet.Test.Server
{
    public class OutOfProcessServer : ITestServer
    {
        public async Task<T> ExecuteAsync<T>(Func<string, Task<T>> action)
        {
            // select the proper server
            string arguments;
            int waitSeconds;
            switch (Mode)
            {
                case TestServerMode.FailedDownload:
                    arguments = $"{TestServerMode.DelayedDownload} -d 120";
                    waitSeconds = 2;
                    break;

                default:
                    throw new InvalidOperationException($"The mode {Mode} is not supported by this server.");
            }

            // start the server
            var process = StartOutOfProcessServer(arguments);

            try
            {
                // read the address, which signals that the server is ready
                var address = process.StandardOutput.ReadLine();

                // execute the action
                var actionTask = action(address);

                // wait for completion
                process.WaitForExit(1000 * waitSeconds);
                if (!process.HasExited)
                {
                    process.Kill();
                }

                // wait for the user's action to complete
                return await actionTask;
            }
            catch (Exception)
            {
                // make sure the external process does not remain
                if (!process.HasExited)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception)
                    {
                        // not much we can do here
                    }
                }
                
                throw;
            }
        }

        private static Process StartOutOfProcessServer(string arguments)
        {
            // build DNX and server paths
            var dnxPath = Path.Combine(PlatformServices.Default.Runtime.RuntimePath, "dnx");

            var appPath = PlatformServices.Default.Application.ApplicationBasePath;
            var basePath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(appPath)));
            var testServerPath = Path.Combine(basePath, "src", "NuGet.Core", "NuGet.Test.Server");

            // start the process
            var process = new Process
            {
                StartInfo =
                {
                    FileName = dnxPath,
                    Arguments = $"-p {testServerPath} run {arguments}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();

            return process;
        }

        public TestServerMode Mode { get; set; } = TestServerMode.FailedDownload;
    }
}
