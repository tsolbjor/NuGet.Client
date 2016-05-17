using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace NuGet.Protocol
{
    public class FindLocalPackagesResourceV2 : FindLocalPackagesResource
    {
        public FindLocalPackagesResourceV2(string root)
        {
            Root = root;
        }

        public override IEnumerable<LocalPackageInfo> FindPackagesById(string id, ILogger logger, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return LocalFolderUtility.GetPackagesV2(Root, logger);
        }

        public override LocalPackageInfo GetPackage(Uri path, ILogger logger, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return LocalFolderUtility.GetPackage(path, logger);
        }

        public override LocalPackageInfo GetPackage(PackageIdentity identity, ILogger logger, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return LocalFolderUtility.GetPackageV2(Root, identity, logger);
        }

        public override IEnumerable<LocalPackageInfo> GetPackages(ILogger logger, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return LocalFolderUtility.GetPackagesV2(Root, logger);
        }
    }
}
