// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using NuGet.ProjectModel;

namespace NuGet.Commands
{
    public class ToolRestoreResult
    {
        public ToolRestoreResult(
            IEnumerable<RestoreTargetGraph> restoreGraphs,
            string lockFilePath,
            LockFile lockFile,
            IEnumerable<CompatibilityCheckResult> checkResults)
        {
            RestoreGraphs = restoreGraphs;
            LockFilePath = lockFilePath;
            LockFile = lockFile;
            CheckResults = checkResults;
        }

        public IEnumerable<RestoreTargetGraph> RestoreGraphs { get; }

        public string LockFilePath { get; }

        public LockFile LockFile { get; }

        public IEnumerable<CompatibilityCheckResult> CheckResults { get; }
    }
}