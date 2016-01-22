using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.Protocol.Core.Types
{
    public class PackageSearchMetadata
    {
        public PackageIdentity Identity { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }

        public Uri IconUrl { get; }

        public IEnumerable<VersionInfo> Versions { get; set; }

        public PackageMetadata LatestPackageMetadata { get; }

        public string Title { get; set; }

        public string Author { get; }

        public long? DownloadCount { get; }
        public string[] Tags { get; set; }
    }
}
