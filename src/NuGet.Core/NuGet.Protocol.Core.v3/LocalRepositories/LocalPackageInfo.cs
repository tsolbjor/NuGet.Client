using NuGet.Packaging;
using NuGet.Packaging.Core;
using System;
using System.IO;

namespace NuGet.Protocol
{
    public class LocalPackageInfo
    {
        /// <summary>
        /// Package id and version.
        /// </summary>
        public PackageIdentity Identity { get; internal set; }

        /// <summary>
        /// Nupkg or folder path.
        /// </summary>
        public string Path { get; internal set; }

        internal Func<PackageReaderBase> PackageHelper { get; set; }

        internal Lazy<NuspecReader> NuspecHelper { get; set; }

        public PackageReaderBase Package
        {
            get
            {
                return PackageHelper();
            }
        }

        public NuspecReader Nuspec
        {
            get
            {
                return NuspecHelper.Value;
            }
        }

        public DateTime LastWriteTimeUtc
        {
            get
            {
                DateTime date = DateTime.UtcNow; ;

                if (File.Exists(Path))
                {
                    return File.GetLastWriteTimeUtc(Path);
                }
                else if (Directory.Exists(Path))
                {
                    return Directory.GetLastWriteTimeUtc(Path);
                }

                return date;
            }
        }

        public bool IsNupkg
        {
            get
            {
                return Path.EndsWith(PackagingCoreConstants.NupkgExtension, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
