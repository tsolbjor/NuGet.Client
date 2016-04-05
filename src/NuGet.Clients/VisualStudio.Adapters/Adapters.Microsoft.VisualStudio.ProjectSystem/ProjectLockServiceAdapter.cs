// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.ProjectSystem
{
    public struct ProjectWriteLockAwaitable
    {
        private global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaitable _instance;

        public ProjectWriteLockAwaitable(global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaitable instance)
        {
            _instance = instance;
        }

        public ProjectWriteLockAwaiter GetAwaiter()
        {
            return new ProjectWriteLockAwaiter(_instance.GetAwaiter());
        }
    }

    public struct ProjectWriteLockAwaiter : System.Runtime.CompilerServices.INotifyCompletion
    {
        private global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaiter _instance;

        public ProjectWriteLockAwaiter(global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaiter instance)
        {
            _instance = instance;
        }

        public bool IsCompleted
        {
            get { return _instance.IsCompleted; }
        }

        public ProjectWriteLockReleaser GetResult()
        {
            return new ProjectWriteLockReleaser(_instance.GetResult());
        }

        public void OnCompleted(Action continuation)
        {
            _instance.OnCompleted(continuation);
        }
    }

    public struct ProjectWriteLockReleaser : IDisposable
    {
        private global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockReleaser _instance;

        public ProjectWriteLockReleaser(global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockReleaser instance)
        {
            _instance = instance;
        }

        public void Dispose()
        {
            _instance.Dispose();
        }

        public Task CheckoutAsync(string file)
        {
            return _instance.CheckoutAsync(file);
        }

        public Task<Adapters.Microsoft.Build.Evaluation.ProjectAdapter> GetProjectAsync(ConfiguredProject configuredProject)
        {
            global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockReleaser instance = _instance;
            return NuGet.VisualStudio.Adapters.Utility.TaskCast<Adapters.Microsoft.Build.Evaluation.ProjectAdapter>(
                () => instance.GetProjectAsync((configuredProject as ConfiguredProjectAdapter).Instance));
        }

        public Task ReleaseAsync()
        {
            return _instance.ReleaseAsync();
        }
    }

    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService))]
    public class ProjectLockServiceAdapter : VsAdapterBase, IProjectLockService
    {
        internal ProjectLockServiceAdapter() { }

        public ProjectLockServiceAdapter(global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService projectLockService) :
            base(projectLockService)
        {
        }

        private global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService)_instance; }
        }

        #region PassThroughs
        //public bool IsAnyLockHeld
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsAnyPassiveLockHeld
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsReadLockHeld
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsPassiveReadLockHeld
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsUpgradeableReadLockHeld
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsPassiveUpgradeableReadLockHeld
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsWriteLockHeld
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsPassiveWriteLockHeld
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public ProjectLockAwaitable ReadLockAsync(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    throw new NotImplementedException();
        //}

        //public ProjectLockAwaitable UpgradeableReadLockAsync(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    throw new NotImplementedException();
        //}

        //public ProjectLockAwaitable UpgradeableReadLockAsync(ProjectLockFlags options, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    throw new NotImplementedException();
        //}

        public ProjectWriteLockAwaitable WriteLockAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return new ProjectWriteLockAwaitable(Instance.WriteLockAsync(cancellationToken));
        }

        //public ProjectWriteLockAwaitable WriteLockAsync(ProjectLockFlags options, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    throw new NotImplementedException();
        //}

        //public ProjectLockSuppression HideLocks()
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
