// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService))]
    public interface IProjectLockService : IVsAdapter
    {
        //bool IsAnyLockHeld { get; }
        //bool IsAnyPassiveLockHeld { get; }
        //bool IsReadLockHeld { get; }
        //bool IsPassiveReadLockHeld { get; }
        //bool IsUpgradeableReadLockHeld { get; }
        //bool IsPassiveUpgradeableReadLockHeld { get; }
        //bool IsWriteLockHeld { get; }
        //bool IsPassiveWriteLockHeld { get; }
        //ProjectLockAwaitable ReadLockAsync(CancellationToken cancellationToken = default(CancellationToken));
        //ProjectLockAwaitable UpgradeableReadLockAsync(CancellationToken cancellationToken = default(CancellationToken));
        //ProjectLockAwaitable UpgradeableReadLockAsync(ProjectLockFlags options, CancellationToken cancellationToken = default(CancellationToken));
        ProjectWriteLockAwaitable WriteLockAsync(CancellationToken cancellationToken = default(CancellationToken));
        //ProjectWriteLockAwaitable WriteLockAsync(ProjectLockFlags options, CancellationToken cancellationToken = default(CancellationToken));
        //ProjectLockSuppression HideLocks();
    }
}
