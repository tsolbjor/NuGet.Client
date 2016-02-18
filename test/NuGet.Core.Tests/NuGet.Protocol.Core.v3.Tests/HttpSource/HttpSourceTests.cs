using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using NuGet.Test.Server;
using NuGet.Test.Utility;
using Xunit;

namespace NuGet.Protocol.Core.v3.Tests
{
    public class HttpSourceTests
    {
        [Fact]
        public async Task HttpSource_ThrowsExceptionWhenJsonDownloadFails()
        {
            // https://github.com/NuGet/Home/issues/2096
            if (!RuntimeEnvironmentHelper.IsWindows)
            {
                return;
            }

            // Arrange
            var server = new OutOfProcessServer { Mode = TestServerMode.FailedDownload };

            using (var target = BuildHttpSource())
            {
                Func<Task> actionAsync = () => server.ExecuteAsync(
                    async address => await target.GetJObjectAsync(
                        new Uri(address),
                        new TestLogger(),
                        CancellationToken.None));

                // Act & Assert
                var exception = await Assert.ThrowsAnyAsync<IOException>(actionAsync);
#if DNXCORE50
                Assert.NotNull(exception.InnerException);
                Assert.Equal("The connection with the server was terminated abnormally", exception.InnerException.Message);
#else
                var innerException = Assert.IsType<SocketException>(exception.InnerException);
                Assert.Equal(SocketError.ConnectionReset, innerException.SocketErrorCode);
#endif
            }
        }

        [Fact]
        public async Task HttpSource_ThrowsExceptionWhenCacheDownloadFails()
        {
            // https://github.com/NuGet/Home/issues/2096
            if (!RuntimeEnvironmentHelper.IsWindows)
            {
                return;
            }

            // Arrange
            var server = new OutOfProcessServer { Mode = TestServerMode.FailedDownload };
            var cacheKey = $"test_{Guid.NewGuid()}";

            using (var target = BuildHttpSource())
            {
                Func<Task> actionAsync = () => server.ExecuteAsync(
                    async address => await target.GetAsync(
                        address,
                        cacheKey,
                        new HttpSourceCacheContext(),
                        new TestLogger(),
                        CancellationToken.None));

                // Act & Assert
                var exception = await Assert.ThrowsAnyAsync<IOException>(actionAsync);
#if DNXCORE50
                Assert.NotNull(exception.InnerException);
                Assert.Equal("The connection with the server was terminated abnormally", exception.InnerException.Message);
#else
                var innerException = Assert.IsType<SocketException>(exception.InnerException);
                Assert.Equal(SocketError.ConnectionReset, innerException.SocketErrorCode);
#endif
            }
        }

        private static HttpSource BuildHttpSource()
        {
            var handler = new HttpHandlerResourceV3(new HttpClientHandler(), new HttpClientHandler());
            var target = new HttpSource(
                new PackageSource("https://api.nuget.org/v3/index.json"),
                () => Task.FromResult((HttpHandlerResource)handler));
            return target;
        }
    }
}
