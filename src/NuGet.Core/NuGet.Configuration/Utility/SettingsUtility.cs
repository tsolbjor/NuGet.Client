﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Common;

namespace NuGet.Configuration
{
    public static class SettingsUtility
    {
        public const string ConfigSection = "config";
        public const string GlobalPackagesFolderKey = "globalPackagesFolder";
        public const string GlobalPackagesFolderEnvironmentKey = "NUGET_PACKAGES";
        public const string RepositoryPathKey = "repositoryPath";
        public static readonly string DefaultGlobalPackagesFolderPath = "packages" + Path.DirectorySeparatorChar;

        public static string GetRepositoryPath(ISettings settings)
        {
            var path = settings.GetValue(ConfigSection, RepositoryPathKey, isPath: true);
            if (!String.IsNullOrEmpty(path))
            {
                path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return path;
        }

        public static string GetDecryptedValue(ISettings settings, string section, string key, bool isPath = false)
        {
            if (String.IsNullOrEmpty(section))
            {
                throw new ArgumentException(Resources.Argument_Cannot_Be_Null_Or_Empty, "section");
            }

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.Argument_Cannot_Be_Null_Or_Empty, "key");
            }

            var encryptedString = settings.GetValue(section, key, isPath);
            if (encryptedString == null)
            {
                return null;
            }

            if (String.IsNullOrEmpty(encryptedString))
            {
                return String.Empty;
            }
            return EncryptionUtility.DecryptString(encryptedString);
        }

        public static void SetEncryptedValue(ISettings settings, string section, string key, string value)
        {
            if (String.IsNullOrEmpty(section))
            {
                throw new ArgumentException(Resources.Argument_Cannot_Be_Null_Or_Empty, "section");
            }

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.Argument_Cannot_Be_Null_Or_Empty, "key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (String.IsNullOrEmpty(value))
            {
                settings.SetValue(section, key, String.Empty);
            }
            else
            {
                var encryptedString = EncryptionUtility.EncryptString(value);
                settings.SetValue(section, key, encryptedString);
            }
        }

        /// <summary>
        /// Retrieves a config value for the specified key
        /// </summary>
        /// <param name="settings">The settings instance to retrieve </param>
        /// <param name="key">The key to look up</param>
        /// <param name="decrypt">Determines if the retrieved value needs to be decrypted.</param>
        /// <param name="isPath">Determines if the retrieved value is returned as a path.</param>
        /// <returns>Null if the key was not found, value from config otherwise.</returns>
        public static string GetConfigValue(ISettings settings, string key, bool decrypt = false, bool isPath = false)
        {
            return decrypt ?
                GetDecryptedValue(settings, ConfigSection, key, isPath) :
                settings.GetValue(ConfigSection, key, isPath);
        }

        /// <summary>
        /// Sets a config value in the setting.
        /// </summary>
        /// <param name="settings">The settings instance to store the key-value in.</param>
        /// <param name="key">The key to store.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="encrypt">Determines if the value needs to be encrypted prior to storing.</param>
        public static void SetConfigValue(ISettings settings, string key, string value, bool encrypt = false)
        {
            if (encrypt == true)
            {
                SetEncryptedValue(settings, ConfigSection, key, value);
            }
            else
            {
                settings.SetValue(ConfigSection, key, value);
            }
        }

        /// <summary>
        /// Deletes a config value from settings
        /// </summary>
        /// <param name="settings">The settings instance to delete the key from.</param>
        /// <param name="key">The key to delete.</param>
        /// <returns>True if the value was deleted, false otherwise.</returns>
        public static bool DeleteConfigValue(ISettings settings, string key)
        {
            return settings.DeleteValue(ConfigSection, key);
        }

        public static string GetGlobalPackagesFolder(ISettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var path = Environment.GetEnvironmentVariable(GlobalPackagesFolderEnvironmentKey);
            if (string.IsNullOrEmpty(path))
            {
                // Environment variable for globalPackagesFolder is not set.
                // Try and get it from nuget settings

                // GlobalPackagesFolder path may be relative path. If so, it will be considered relative to
                // the solution directory, just like the 'repositoryPath' setting
                path = settings.GetValue(ConfigSection, GlobalPackagesFolderKey, isPath: false);
            }

            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                return path;
            }

            path = Path.Combine(NuGetEnvironment.GetFolderPath(NuGetFolderPath.NuGetHome), DefaultGlobalPackagesFolderPath);

            return path;
        }

        public static string GetHttpCacheFolder(ISettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            
            return NuGetEnvironment.GetFolderPath(NuGetFolderPath.HttpCacheDirectory);
        }

        /// <summary>
        /// The DefaultPushSource can be:
        /// - An absolute URL
        /// - An absolute file path
        /// - A relative file path
        /// - The name of a registered source from a config file
        /// </summary>
        public static string GetDefaultPushSource(ISettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            string source = settings.GetValue(ConfigurationConstants.Config, ConfigurationConstants.DefaultPushSource, isPath: false);

            Uri sourceUri = UriUtility.TryCreateSourceUri(source, UriKind.RelativeOrAbsolute);
            if (sourceUri != null && !sourceUri.IsAbsoluteUri)
            {
                // For non-absolute sources, it could be the name of a config source, or a relative file path.
                IPackageSourceProvider sourceProvider = new PackageSourceProvider(settings);
                IEnumerable<PackageSource> allSources = sourceProvider.LoadPackageSources();

                if (!allSources.Any(s => s.IsEnabled && s.Name.Equals(source, StringComparison.OrdinalIgnoreCase)))
                {
                    // It wasn't the name of a source, so treat it like a relative file path
                    source = settings.GetValue(ConfigurationConstants.Config, ConfigurationConstants.DefaultPushSource, isPath: true);
                }
            }

            return source;
        }

        /// <summary>
        /// Converts "resolvePath" to an absolute file path, relative to "rootDirectory".
        /// </summary>
        public static string ResolvePath(string rootDirectory, string resolvePath)
        {
            if (string.IsNullOrEmpty(rootDirectory))
            {
                return resolvePath;
            }

            // Three cases for when Path.IsRooted(value) is true:
            // 1- C:\folder\file
            // 2- \\share\folder\file
            // 3- \folder\file
            // In the first two cases, we want to honor the fully qualified path
            // In the last case, we want to return X:\folder\file with X: drive where config file is located.
            // However, Path.Combine(path1, path2) always returns path2 when Path.IsRooted(path2) == true (which is current case)
            string root = Path.GetPathRoot(resolvePath);

            // this corresponds to 3rd case
            if (root != null && root.Length == 1 &&
                (root[0] == Path.DirectorySeparatorChar || resolvePath[0] == Path.AltDirectorySeparatorChar))
            {
                return Path.Combine(Path.GetPathRoot(rootDirectory), resolvePath.Substring(1));
            }

            return Path.Combine(rootDirectory, resolvePath);
        }
    }
}
