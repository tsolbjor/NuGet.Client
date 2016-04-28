using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace NuGet.PackageManagement
{
    public class PackagePreFetcherResult : IDisposable
    {
        private readonly Task<DownloadResourceResult> _downloadTask;
        private DownloadResourceResult _result;
        private ExceptionDispatchInfo _exception;

        /// <summary>
        /// True if the result came from the packages folder.
        /// </summary>
        /// <remarks>Not thread safe</remarks>
        public bool InPackagesFolder { get; }

        /// <summary>
        /// Package identity.
        /// </summary>
        public PackageIdentity Package { get; }

        /// <summary>
        /// PackageSource for the download. This is null if the packages folder was used.
        /// </summary>
        public Configuration.PackageSource Source { get; }

        /// <summary>
        /// True if the download is complete.
        /// </summary>
        public bool IsComplete { get; private set; }

        /// <summary>
        /// Create a PreFetcher result for a downloaded package.
        /// </summary>
        public PackagePreFetcherResult(
            Task<DownloadResourceResult> downloadTask,
            PackageIdentity package,
            Configuration.PackageSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (downloadTask == null)
            {
                throw new ArgumentNullException(nameof(downloadTask));
            }

            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _downloadTask = downloadTask;
            Package = package;
            InPackagesFolder = false;
            Source = source;
        }

        /// <summary>
        /// Create a PreFetcher result for a package in the packages folder.
        /// </summary>
        public PackagePreFetcherResult(
            string nupkgPath,
            PackageIdentity package)
        {
            if (nupkgPath == null)
            {
                throw new ArgumentNullException(nameof(nupkgPath));
            }

            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            InPackagesFolder = true;

            // Create a download result for the package that already exists
            _result = new DownloadResourceResult(
                File.OpenRead(nupkgPath),
                new PackageArchiveReader(nupkgPath));
        }

        /// <summary>
        /// A safe wait for the download task. Exceptions are caught and stored.
        /// </summary>
        public async Task EnsureResultAsync()
        {
            if (_result == null)
            {
                try
                {
                    _result = await _downloadTask;
                }
                catch (Exception ex)
                {
                    _exception = ExceptionDispatchInfo.Capture(ex);
                }

                IsComplete = true;
            }
        }

        /// <summary>
        /// Ensure and retrieve the download result.
        /// </summary>
        public async Task<DownloadResourceResult> GetResultAsync()
        {
            await EnsureResultAsync();

            if (_exception != null)
            {
                // Rethrow the exception if the download failed
                _exception.Throw();
            }

            return _result;
        }

        public void Dispose()
        {
            // The task should be awaited before calling dispose
            if (_result != null)
            {
                _result.Dispose();
            }
        }
    }
}
