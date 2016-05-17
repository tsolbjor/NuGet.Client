// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.Protocol
{
    public static class LocalFolderUtility
    {
        public static CachedPackageInfo GetPackage(string root, PackageIdentity packageIdentity, ILogger log)
        {
            return GetPackage(root, packageIdentity.Id, packageIdentity.Version, log);
        }

        public static CachedPackageInfo GetPackage(string root, string id, NuGetVersion version, ILogger log)
        {
            // TODO: Optimize this and bring back the older version fallback search
            // A comparer should be written that contains the legacy fallback logic
            return GetPackageInfos(root, id, log).FirstOrDefault(info => info.Reader.GetVersion() == version);
        }

        /// <summary>
        /// Get package infos from a local folder. Used for project.json restore.
        /// </summary>
        /// <param name="root">Folder root.</param>
        /// <param name="id">Package id.</param>
        public static List<CachedPackageInfo> GetPackageInfos(string root, string id, ILogger log)
        {
            var result = new List<CachedPackageInfo>();

            // packages\{packageId}.{version}.nupkg
            foreach (var nupkgInfo in GetNupkgsFromFlatFolder(root, id, log))
            {
                using (var stream = nupkgInfo.OpenRead())
                using (var packageReader = new PackageArchiveReader(stream))
                {
                    NuspecReader reader;
                    try
                    {
                        using (var nuspecStream = packageReader.GetNuspec())
                        {
                            reader = new NuspecReader(nuspecStream);
                        }
                    }
                    catch (XmlException ex)
                    {
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            Strings.Protocol_PackageMetadataError,
                            nupkgInfo.Name,
                            root);

                        throw new FatalProtocolException(message, ex);
                    }
                    catch (PackagingException ex)
                    {
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            Strings.Protocol_PackageMetadataError,
                            nupkgInfo.Name,
                            root);

                        throw new FatalProtocolException(message, ex);
                    }

                    if (string.Equals(reader.GetId(), id, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(new CachedPackageInfo { Path = nupkgInfo.FullName, Reader = reader });
                    }
                }
            }

            return result;
        }

        public static LocalPackageInfo GetPackage(Uri path, ILogger log)
        {
            return GetPackageFromNupkg(new FileInfo(path.LocalPath));
        }

        public static IEnumerable<LocalPackageInfo> GetPackagesV2(string root, ILogger log)
        {
            return GetPackagesFromNupkgs(GetNupkgsFromFlatFolder(root, log));
        }

        public static IEnumerable<LocalPackageInfo> GetPackagesV2(string root, string id, ILogger log)
        {
            return GetPackagesFromNupkgs(GetNupkgsFromFlatFolder(root, id, log));
        }

        public static LocalPackageInfo GetPackageV2(string root, string id, NuGetVersion version, ILogger log)
        {
            return GetPackageV2(root, new PackageIdentity(id, version), log);
        }

        public static LocalPackageInfo GetPackageV2(string root, PackageIdentity identity, ILogger log)
        {
            var possiblePaths = PackagePathHelper.GetPackageLookupPaths(identity, new PackagePathResolver(root))
                .Where(path => path.EndsWith(PackagingCoreConstants.NupkgExtension, StringComparison.OrdinalIgnoreCase)
                    && File.Exists(path))
                .Select(path => new FileInfo(path));

            return GetPackagesFromNupkgs(possiblePaths)
                .FirstOrDefault(package => identity.Equals(package.Identity));
        }

        public static LocalPackageInfo GetPackageV3(string root, string id, NuGetVersion version, ILogger log)
        {
            return GetPackageV3(root, new PackageIdentity(id, version), log);
        }

        public static LocalPackageInfo GetPackageV3(string root, PackageIdentity identity, ILogger log)
        {
            var pathResolver = new VersionFolderPathResolver(root);

            var nupkgPath = pathResolver.GetPackageFilePath(identity.Id, identity.Version);

            if (File.Exists(nupkgPath) && IsValidForV3(pathResolver, identity.Id, identity.Version))
            {
                var nuspecPath = pathResolver.GetManifestFileName(identity.Id, identity.Version);

                var packageHelper = new Func<PackageReaderBase>(() => new PackageArchiveReader(nupkgPath));

                var nuspecHelper = new Lazy<NuspecReader>(() => {
                    if (File.Exists(nuspecPath))
                    {
                        return new NuspecReader(nuspecPath);
                    }
                    else
                    {
                        using (var packageReader = packageHelper())
                        {
                            return packageReader.NuspecReader;
                        }
                    }
                });

                return new LocalPackageInfo()
                {
                    Identity = new PackageIdentity(identity.Id, identity.Version),
                    Path = nupkgPath,
                    NuspecHelper = nuspecHelper,
                    PackageHelper = packageHelper
                };
            }

            return null;
        }

        private static IEnumerable<LocalPackageInfo> GetPackagesFromNupkgs(IEnumerable<FileInfo> files)
        {
            return files.Select(GetPackageFromNupkg);
        }

        private static LocalPackageInfo GetPackageFromNupkg(FileInfo nupkgFile)
        {
            using (var package = new PackageArchiveReader(nupkgFile.FullName))
            {
                var nuspec = package.NuspecReader;

                var packageHelper = new Func<PackageReaderBase>(() => new PackageArchiveReader(nupkgFile.FullName));
                var nuspecHelper = new Lazy<NuspecReader>(() => nuspec);

                return new LocalPackageInfo()
                {
                    Identity = nuspec.GetIdentity(),
                    Path = nupkgFile.FullName,
                    NuspecHelper = nuspecHelper,
                    PackageHelper = packageHelper
                };
            }
        }

        /// <summary>
        /// Discover all nupkgs from a v2 local folder.
        /// </summary>
        /// <param name="root">Folder root.</param>
        public static IEnumerable<FileInfo> GetNupkgsFromFlatFolder(string root, ILogger log)
        {
            // Match all nupkgs in the folder
            return GetNupkgsFromFlatFolder(root, id: string.Empty, log: log);
        }

        /// <summary>
        /// Discover nupkgs from a v2 local folder.
        /// </summary>
        /// <param name="root">Folder root.</param>
        /// <param name="id">Package id or package id prefix.</param>
        public static IEnumerable<FileInfo> GetNupkgsFromFlatFolder(string root, string id, ILogger log)
        {
            // Check for package files one level deep.
            DirectoryInfo rootDirectoryInfo = null;

            try
            {
                rootDirectoryInfo = new DirectoryInfo(root);
            }
            catch (ArgumentException ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Strings.Log_FailedToRetrievePackage, root);

                throw new FatalProtocolException(message, ex);
            }

            if (rootDirectoryInfo == null || !Directory.Exists(rootDirectoryInfo.FullName))
            {
                yield break;
            }

            var filter = $"*{NuGetConstants.PackageExtension}";

            // Check top level directory
            foreach (var path in rootDirectoryInfo.EnumerateFiles(filter))
            {
                if (path.Name.StartsWith(id, StringComparison.OrdinalIgnoreCase))
                {
                    yield return path;
                }
            }

            // Check sub directories
            foreach (var dir in rootDirectoryInfo.EnumerateDirectories(filter))
            {
                foreach (var path in dir.EnumerateFiles(filter))
                {
                    if (path.Name.StartsWith(id, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return path;
                    }
                }
            }
        }

        /// <summary>
        /// True if a nupkg exists in the root or the first level of sub folders.
        /// </summary>
        public static bool IsV2FolderStructure(string root, ILogger log)
        {
            return GetNupkgsFromFlatFolder(root, log).Any();
        }

        /// <summary>
        /// True if this is a v3 folder format.
        /// </summary>
        public static bool IsV3FolderStructure(string root, ILogger log)
        {
            // Treat missing directories as v2
            if (Directory.Exists(root))
            {
                return false;
            }

            // Check for nupkgs in the root or sub folders.
            if (IsV2FolderStructure(root, log))
            {
                return false;
            }

            var rootDir = new DirectoryInfo(root);
            var pathResolver = new VersionFolderPathResolver(root);

            // Check for an actual V3 file
            foreach (var idDir in GetDirectoriesSafe(root, log))
            {
                foreach (var versionDir in GetDirectoriesSafe(root, log))
                {
                    var id = Path.GetDirectoryName(idDir);
                    var versionString = Path.GetDirectoryName(versionDir);

                    NuGetVersion version;
                    if (NuGetVersion.TryParse(versionString, out version))
                    {
                        var hashPath = pathResolver.GetHashPath(id, version);

                        if (File.Exists(hashPath))
                        {
                            // If we have files in the format {packageId}/{version}/{packageId}.nupkg.sha512, assume it's an expanded package repository.
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Discover all nupkgs from a v3 folder.
        /// </summary>
        /// <param name="root">Folder root.</param>
        public static IEnumerable<LocalPackageInfo> GetPackagesV3(string root, ILogger log)
        {
            // Match all nupkgs in the folder
            foreach (var id in GetDirectoriesSafe(root, log))
            {
                foreach (var nupkg in GetPackagesV3(root, id: id, log: log))
                {
                    yield return nupkg;
                }
            }

            yield break;
        }

        /// <summary>
        /// Discover nupkgs from a v3 local folder.
        /// </summary>
        /// <param name="root">Folder root.</param>
        /// <param name="id">Package id or package id prefix.</param>
        public static IEnumerable<LocalPackageInfo> GetPackagesV3(string root, string id, ILogger log)
        {
            // Check for package files one level deep.
            DirectoryInfo rootDirectoryInfo = null;

            try
            {
                rootDirectoryInfo = new DirectoryInfo(root);
            }
            catch (ArgumentException ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Strings.Log_FailedToRetrievePackage, root);

                throw new FatalProtocolException(message, ex);
            }

            if (rootDirectoryInfo == null || !Directory.Exists(rootDirectoryInfo.FullName))
            {
                // Directory is missing
                yield break;
            }

            var pathResolver = new VersionFolderPathResolver(root);
            var idRoot = Path.Combine(root, id);

            foreach (var versionDir in GetDirectoriesSafe(idRoot, log))
            {
                NuGetVersion version;
                if (NuGetVersion.TryParse(versionDir, out version))
                {
                    var nupkgPath = pathResolver.GetPackageFileName(id, version);

                    // This may not exist if another thread is currently extracting the package, there is no reason to warn here.
                    if (File.Exists(nupkgPath) && IsValidForV3(pathResolver, id, version))
                    {
                        var nuspecPath = pathResolver.GetManifestFileName(id, version);

                        var packageHelper = new Func<PackageReaderBase>(() => new PackageArchiveReader(nupkgPath));

                        var nuspecHelper = new Lazy<NuspecReader>(() => {
                                if (File.Exists(nuspecPath))
                                {
                                    return new NuspecReader(nuspecPath);
                                }
                                else
                                {
                                    using (var packageReader = packageHelper())
                                    {
                                        return packageHelper().NuspecReader;
                                    }
                                }
                            });

                        yield return new LocalPackageInfo()
                        {
                            Identity = new PackageIdentity(id, version),
                            Path = nupkgPath,
                            NuspecHelper = nuspecHelper,
                            PackageHelper = packageHelper
                        };
                    }
                }
                else
                {
                    log.LogWarning($"Unable to parse version: {versionDir}");
                }
            }

            yield break;
        }

        private static IEnumerable<string> GetDirectoriesSafe(string root, ILogger log)
        {
            try
            {
                return Directory.GetDirectories(root);
            }
            catch (Exception e)
            {
                log.LogWarning(e.Message);
            }

            return Enumerable.Empty<string>();
        }

        private static IEnumerable<string> GetFilesSafe(string root, string filter, ILogger log)
        {
            try
            {
                return Directory.GetFiles(root, filter);
            }
            catch (Exception e)
            {
                log.LogWarning(e.Message);
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// True if the hash file exists.
        /// </summary>
        private static bool IsValidForV3(VersionFolderPathResolver pathResolver, string id, NuGetVersion version)
        {
            var hashPath = pathResolver.GetHashPath(id, version);

            return File.Exists(hashPath);
        }
    }
}
