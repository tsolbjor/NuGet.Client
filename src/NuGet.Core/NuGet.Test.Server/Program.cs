using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NuGet.Test.Server
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return MainAsync(args).Result;
        }

        private static async Task<int> MainAsync(string[] args)
        {
            ITestServer server;
            if (!ParseTestServer(args, out server))
            {
                return 1;
            }

            await server.ExecuteAsync(
                address =>
                {
                    // write the address and wait
                    Console.WriteLine(address);
                    Console.ReadKey();

                    return Task.FromResult((object)null);
                });

            return 0;
        }

        private static bool ParseTestServer(string[] args, out ITestServer server)
        {
            server = null;

            if (args.Length < 1)
            {
                Console.Error.WriteLine("There must be at least one argument.");
                Console.Error.WriteLine();
                WriteUsage();
                return false;
            }

            TestServerMode mode;
            if (!Enum.TryParse(args[0], true, out mode))
            {
                Console.Error.WriteLine("The mode could not be parsed.");
                Console.Error.WriteLine();
                WriteUsage();
                return false;
            }

            switch (mode)
            {
                case TestServerMode.DelayedDownload:
                    var kestrelServer = new KestrelServer();

                    var unparsedDelay = GetOption(args, "-d");
                    if (unparsedDelay != null)
                    {
                        int parsedDelay;
                        if (!int.TryParse(unparsedDelay, out parsedDelay))
                        {
                            Console.Error.WriteLine("The -d option could not be parsed as an integer.");
                            Console.Error.WriteLine();
                            WriteUsage();
                            return false;
                        }

                        kestrelServer.DownloadDelay = TimeSpan.FromSeconds(parsedDelay);
                    }

                    server = kestrelServer;
                    return true;

                default:
                    Console.Error.WriteLine($"The mode {mode} is not supported.");
                    Console.Error.WriteLine();
                    WriteUsage();
                    return false;
            }
        }

        private static string GetOption(string[] args, string name)
        {
            return args
                .SkipWhile(arg => !StringComparer.OrdinalIgnoreCase.Equals(arg, name))
                .Skip(1)
                .FirstOrDefault();
        }

        private static void WriteUsage()
        {
            Console.Error.WriteLine(@"Usage: NuGet.Test.Server MODE [OPTIONS]

When the server has started, the server address will be printed to STDOUT. The
program will hang until a single character has been sent to STDIN. The
supported modes and their respective options are listed below. 

MODE: DelayedDownload

  Delay in the middle of sending the response.

  Options:
    
    -d   The time in seconds to delay. The default is 10 seconds.");
        }
    }
}
