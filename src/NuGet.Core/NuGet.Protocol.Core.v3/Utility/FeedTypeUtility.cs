using System;
using System.IO;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;

namespace NuGet.Protocol
{
    public static class FeedTypeUtility
    {
        public static FeedType GetFeedType(PackageSource packageSource)
        {
            // Default to unknown file system
            var type = FeedType.FileSystemUnknown;

            if (packageSource.IsHttp)
            {
                if (packageSource.Source.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    type = FeedType.HttpV3;
                }
                else
                {
                    type = FeedType.HttpV2;
                }
            }
            else if (packageSource.IsLocal)
            {
                var path = packageSource.Source;

                if (!Directory.Exists(path))
                {
                    // If the directory doesn't exist check again later
                    type = FeedType.FileSystemUnknown;
                }
                else
                {
                    if (LocalFolderUtility.IsV3FolderStructure(path, NullLogger.Instance))
                    {
                        type = FeedType.FileSystemV3;
                    }
                    else
                    {
                        // Use v2 if the folder does not exist yet
                        type = FeedType.FileSystemV2;
                    }
                }
            }

            return type;
        }
    }
}
