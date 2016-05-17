// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.Protocol
{
    public class LocalV2FindPackageByIdResource : FindPackageByIdResource
    {
        private readonly ConcurrentDictionary<string, List<CachedPackageInfo>> _packageInfoCache;
        private readonly string _source;

        public LocalV2FindPackageByIdResource(PackageSource packageSource,
            ConcurrentDictionary<string, List<CachedPackageInfo>> packageInfoCache)
        {
            _source = packageSource.Source;
            _packageInfoCache = packageInfoCache;
        }

        public override Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string id, CancellationToken token)
        {
            var infos = GetPackageInfos(id);
            return Task.FromResult(infos.Select(p => p.Reader.GetVersion()));
        }

        public override Task<Stream> GetNupkgStreamAsync(string id, NuGetVersion version, CancellationToken token)
        {
            var info = GetPackageInfo(id, version);
            if (info != null)
            {
                return Task.FromResult<Stream>(File.OpenRead(info.Path));
            }

            return Task.FromResult<Stream>(null);
        }

        public override Task<FindPackageByIdDependencyInfo> GetDependencyInfoAsync(string id, NuGetVersion version, CancellationToken token)
        {
            FindPackageByIdDependencyInfo dependencyInfo = null;
            var info = GetPackageInfo(id, version);
            if (info != null)
            {
                dependencyInfo = GetDependencyInfo(info.Reader);
            }

            return Task.FromResult(dependencyInfo);
        }

        private CachedPackageInfo GetPackageInfo(string id, NuGetVersion version)
        {
            return GetPackageInfos(id).FirstOrDefault(package => package.Reader.GetVersion() == version);
        }

        private List<CachedPackageInfo> GetPackageInfos(string id)
        {
            List<CachedPackageInfo> result;

            if (!_packageInfoCache.TryGetValue(id, out result))
            {
                // TODO: optimize this
                if (!Directory.Exists(_source))
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Strings.Log_FailedToRetrievePackage, _source);

                    throw new FatalProtocolException(message);
                }

                result = LocalFolderUtility.GetPackageInfos(_source, id, Logger);

                _packageInfoCache.TryAdd(id, result);
            }

            return result;
        }
    }
}
