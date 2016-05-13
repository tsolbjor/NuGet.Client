// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.Protocol.Core.v3.RemoteRepositories
{
    /// <summary>
    /// A <see cref="FindPackageByIdResource" /> for a Http-based file system where files are laid out in the
    /// format
    /// /root/
    /// PackageA/
    /// Version0/
    /// PackageA.nuspec
    /// PackageA.Version0.nupkg
    /// and are accessible via HTTP Gets.
    /// </summary>
    public class HttpFileSystemBasedFindPackageByIdResource : FindPackageByIdResource
    {
        private const int MaxRetries = 3;
        private readonly HttpSource _httpSource;
        private readonly ConcurrentDictionary<string, Task<SortedDictionary<NuGetVersion, PackageInfo>>> _packageInfoCache =
            new ConcurrentDictionary<string, Task<SortedDictionary<NuGetVersion, PackageInfo>>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Task<NupkgEntry>> _nupkgCache = new Dictionary<string, Task<NupkgEntry>>();
        private readonly IReadOnlyList<Uri> _baseUris;

        public HttpFileSystemBasedFindPackageByIdResource(
            IReadOnlyList<Uri> baseUris,
            HttpSource httpSource)
        {
            if (baseUris == null)
            {
                throw new ArgumentNullException(nameof(baseUris));
            }

            if (baseUris.Count < 1)
            {
                throw new ArgumentException(Strings.OneOrMoreUrisMustBeSpecified, nameof(baseUris));
            }

            _baseUris = baseUris
                .Take(MaxRetries)
                .Select(uri => uri.OriginalString.EndsWith("/", StringComparison.Ordinal) ? uri : new Uri(uri.OriginalString + "/"))
                .ToList();

            _httpSource = httpSource;
        }

        public override ILogger Logger
        {
            get { return base.Logger; }
            set
            {
                base.Logger = value;
            }
        }

        public override async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string id, CancellationToken cancellationToken)
        {
            var packageInfos = await EnsurePackagesAsync(id, cancellationToken);
            return packageInfos.Keys;
        }

        public override async Task<FindPackageByIdDependencyInfo> GetDependencyInfoAsync(string id, NuGetVersion version, CancellationToken cancellationToken)
        {
            NuGetEventSource.Log.Load(10, $"HTTP GetNupkgStreamAsync2 {id} {version} start");

            var packageInfos = await EnsurePackagesAsync(id, cancellationToken);

            PackageInfo packageInfo;
            if (packageInfos.TryGetValue(version, out packageInfo))
            {
                var reader = await PackageUtilities.OpenNuspecFromNupkgAsync(
                packageInfo.Id,
                OpenNupkgStreamAsync(packageInfo, cancellationToken),
                Logger);

                NuGetEventSource.Log.Load(10, $"HTTP GetNupkgStreamAsync2 {id} {version} end");
                return GetDependencyInfo(reader);
            }

            NuGetEventSource.Log.Load(10, $"HTTP GetNupkgStreamAsync2 {id} {version} end");
            return null;
        }

        public override async Task<Stream> GetNupkgStreamAsync(string id, NuGetVersion version, CancellationToken cancellationToken)
        {
            NuGetEventSource.Log.Load(10, $"HTTP GetNupkgStreamAsync {id} {version} start");

            var packageInfos = await EnsurePackagesAsync(id, cancellationToken);

            PackageInfo packageInfo;
            if (packageInfos.TryGetValue(version, out packageInfo))
            {
                NuGetEventSource.Log.Load(10, $"HTTP GetNupkgStreamAsync {id} {version} end");

                return await OpenNupkgStreamAsync(packageInfo, cancellationToken);
            }

            return null;
        }

        private Task<SortedDictionary<NuGetVersion, PackageInfo>> EnsurePackagesAsync(string id, CancellationToken cancellationToken)
        {
            return _packageInfoCache.GetOrAdd(id, (keyId) => FindPackagesByIdAsync(keyId, cancellationToken));
        }

        private async Task<SortedDictionary<NuGetVersion, PackageInfo>> FindPackagesByIdAsync(string id, CancellationToken cancellationToken)
        {
            NuGetEventSource.Log.Load(10, $"HTTP FindPackagesByIdAsync {id} start");

            for (var retry = 0; retry != 3; ++retry)
            {
                var baseUri = _baseUris[retry % _baseUris.Count].OriginalString;
                var uri = baseUri + id.ToLowerInvariant() + "/index.json";

                try
                {
                    using (var result = await _httpSource.GetAsync(
                        uri,
                        $"list_{id}",
                        CreateCacheContext(retry),
                        Logger,
                        ignoreNotFounds: true,
                        ensureValidContents: stream => HttpStreamValidation.ValidateJObject(uri, stream),
                        cancellationToken: cancellationToken))
                    {
                        NuGetEventSource.Log.Load(10, $"HTTP FindPackagesByIdAsync {id} end");

                        if (result.Status == HttpSourceResultStatus.NotFound)
                        {
                            return new SortedDictionary<NuGetVersion, PackageInfo>();
                        }

                        try
                        {
                            return ConsumeFlatContainerIndex(result.Stream, id, baseUri);
                        }
                        catch
                        {
                            Logger.LogWarning(string.Format(CultureInfo.CurrentCulture, Strings.Log_FileIsCorrupt, result.CacheFileName));

                            throw;
                        }
                    }
                }
                catch (Exception ex) when (retry < 2)
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Strings.Log_RetryingFindPackagesById, nameof(FindPackagesByIdAsync), uri)
                        + Environment.NewLine
                        + ExceptionUtilities.DisplayMessage(ex);
                    Logger.LogMinimal(message);
                }
                catch (Exception ex) when (retry == 2)
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Strings.Log_FailedToRetrievePackage, uri);
                    Logger.LogError(message + Environment.NewLine + ExceptionUtilities.DisplayMessage(ex));

                    throw new FatalProtocolException(message, ex);
                }
            }

            NuGetEventSource.Log.Load(10, $"HTTP FindPackagesByIdAsync {id} end");

            return null;
        }

        private SortedDictionary<NuGetVersion, PackageInfo> ConsumeFlatContainerIndex(Stream stream, string id, string baseUri)
        {
            JObject doc;
            using (var reader = new StreamReader(stream))
            {
                doc = JObject.Load(new JsonTextReader(reader));
            }

            var streamResults = new SortedDictionary<NuGetVersion, PackageInfo>();

            var versions = doc["versions"];
            if (versions == null)
            {
                return streamResults;
            }

            foreach (var packageInfo in versions
                .Select(x => BuildModel(baseUri, id, x.ToString()))
                .Where(x => x != null))
            {
                if (!streamResults.ContainsKey(packageInfo.Version))
                {
                    streamResults.Add(packageInfo.Version, packageInfo);
                }
            }

            return streamResults;
        }

        private PackageInfo BuildModel(string baseUri, string id, string version)
        {
            var parsedVersion = NuGetVersion.Parse(version);
            var normalizedVersionString = parsedVersion.ToNormalizedString();
            return new PackageInfo
            {
                // If 'Id' element exist, use its value as accurate package Id
                // Otherwise, use the value of 'title' if it exist
                // Use the given Id as final fallback if all elements above don't exist
                Id = id,
                Version = parsedVersion,
                ContentUri = baseUri + id.ToLowerInvariant() + "/" + normalizedVersionString + "/" + id.ToLowerInvariant() + "." + normalizedVersionString + ".nupkg",
            };
        }

        private async Task<Stream> OpenNupkgStreamAsync(PackageInfo package, CancellationToken cancellationToken)
        {
            NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} start");

            NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} before lock");

            Task<NupkgEntry> task;
            lock (_nupkgCache)
            {
                NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} lock start");

                if (!_nupkgCache.TryGetValue(package.ContentUri, out task))
                {
                    NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} task start");
                    task = _nupkgCache[package.ContentUri] = OpenNupkgStreamAsyncCore(package, cancellationToken);
                    NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} task end");
                }

                NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} lock end");
            }

            NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} await task start");
            var result = await task;
            if (result == null)
            {
                NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} await task null end");
                return null;
            }

            NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} await task end");

            // Acquire the lock on a file before we open it to prevent this process
            // from opening a file deleted by the logic in HttpSource.GetAsync() in another process
            return await ConcurrencyUtilities.ExecuteWithFileLockedAsync(result.TempFileName,
                action: token =>
                {
                    NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} end");

                    return Task.FromResult(
                        new FileStream(result.TempFileName,
                                       FileMode.Open,
                                       FileAccess.Read,
                                       FileShare.ReadWrite | FileShare.Delete));
                },
                token: cancellationToken);
        }

        private async Task<NupkgEntry> OpenNupkgStreamAsyncCore(PackageInfo package, CancellationToken cancellationToken)
        {
            NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} start");

            for (var retry = 0; retry != 3; ++retry)
            {
                try
                {
                    using (var data = await _httpSource.GetAsync(
                        package.ContentUri,
                        "nupkg_" + package.Id + "." + package.Version.ToNormalizedString(),
                        CreateCacheContext(retry),
                        Logger,
                        ignoreNotFounds: false,
                        ensureValidContents: stream => HttpStreamValidation.ValidateNupkg(package.ContentUri, stream),
                        cancellationToken: cancellationToken))
                    {
                        NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} success end");

                        return new NupkgEntry
                        {
                            TempFileName = data.CacheFileName
                        };
                    }
                }
                catch (Exception ex) when (retry < 2)
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Strings.Log_FailedToDownloadPackage, package.ContentUri)
                        + Environment.NewLine
                        + ExceptionUtilities.DisplayMessage(ex); 
                    Logger.LogMinimal(message);
                }
                catch (Exception ex) when (retry == 2)
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Strings.Log_FailedToDownloadPackage, package.ContentUri)
                        + Environment.NewLine
                        + ExceptionUtilities.DisplayMessage(ex);
                    Logger.LogError(message);
                }
            }

            NuGetEventSource.Log.Load(10, $"HTTP OpenNupkgStreamAsync {package.ContentUri} null end");

            return null;
        }

        private class NupkgEntry
        {
            public string TempFileName { get; set; }
        }

        private class PackageInfo
        {
            public string Id { get; set; }

            public string Path { get; set; }

            public string ContentUri { get; set; }

            public NuGetVersion Version { get; set; }
        }
    }
}
