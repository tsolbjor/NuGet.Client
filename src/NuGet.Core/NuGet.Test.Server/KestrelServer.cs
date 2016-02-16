using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;

namespace NuGet.Test.Server
{
    public class KestrelServer : ITestServer
    {
        public async Task<T> ExecuteAsync<T>(Func<string, Task<T>> action)
        {
            RequestDelegate handler;
            switch (Mode)
            {
                case TestServerMode.DelayedDownload:
                    handler = DelayedDownloadHandler;
                    break;

                default:
                    throw new InvalidOperationException($"The mode {Mode} is not supported by this server.");
            }

            var portReserver = new PortReserver();
            return await portReserver.ExecuteAsync(
                async (port, token) =>
                {
                    var address = $"http://localhost:{port}/";
                    var config = new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string>
                        {
                            { "server.urls", address }
                        })
                        .Build();

                    var hostBuilder = new WebHostBuilder(config)
                        .UseServer("Microsoft.AspNet.Server.Kestrel")
                        .UseStartup(appBuilder =>
                        {
                            appBuilder.Run(handler);
                        });
                    
                    using (hostBuilder.Build().Start())
                    {
                        return await action(address);
                    }
                },
                CancellationToken.None);
        }

        public TimeSpan DownloadDelay { get; set; } = TimeSpan.FromSeconds(5);

        private async Task DelayedDownloadHandler(HttpContext context)
        {
            var before = @"{""foo"": 1,";
            var after = @" ""bar"": 2}";

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength = before.Length + after.Length;

            await context.Response.WriteAsync(before);
            await context.Response.Body.FlushAsync();

            await Task.Delay(DownloadDelay);

            await context.Response.WriteAsync(after);
        }

        public TestServerMode Mode { get; set; } = TestServerMode.DelayedDownload;
    }
}
