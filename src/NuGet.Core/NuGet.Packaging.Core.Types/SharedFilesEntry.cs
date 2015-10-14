﻿using System;

namespace NuGet.Packaging.Core
{
    /// <summary>
    /// metadata/shared/files entry from a nuspec
    /// </summary>
    public class SharedFilesEntry
    {
        /// <summary>
        /// Included files
        /// </summary>
        /// <remarks>Required</remarks>
        public string Include { get; }

        /// <summary>
        /// Excluded files
        /// </summary>
        public string Exclude { get; }

        /// <summary>
        /// Build action
        /// </summary>
        public string BuildAction { get; }

        /// <summary>
        /// If true the item will be copied to the output folder.
        /// </summary>
        public bool? CopyToOutput { get; }

        /// <summary>
        /// If true the content items will keep the same folder structure in the output
        /// folder.
        /// </summary>
        public bool? Flatten { get; }

        public SharedFilesEntry(
            string include,
            string exclude,
            string buildAction,
            bool? copyToOutput,
            bool? flatten)
        {
            if (include == null)
            {
                throw new ArgumentNullException(nameof(include));
            }

            Include = include;
            Exclude = exclude;
            BuildAction = buildAction;
            CopyToOutput = copyToOutput;
            Flatten = flatten;
        }
    }
}