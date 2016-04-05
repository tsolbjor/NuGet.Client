// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.CompilerServices;
using NuGet.VisualStudio.Adapters;
using Adapters.Microsoft.VisualStudio.Threading;

namespace Adapters.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.IThreadHandling))]
    public class ThreadHandlingAdapter : VsAdapterBase, IThreadHandling
    {
        internal ThreadHandlingAdapter() { }

        public ThreadHandlingAdapter(global::Microsoft.VisualStudio.ProjectSystem.IThreadHandling threadHandling) :
            base(threadHandling)
        {
        }

        private global::Microsoft.VisualStudio.ProjectSystem.IThreadHandling Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.IThreadHandling)_instance; }
        }

        #region PassThroughs
        //public JoinableTaskContextNode JoinableTaskContext
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public JoinableTaskFactory AsyncPump
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsOnMainThread
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public JoinableTaskFactory.MainThreadAwaitable SwitchToUIThread(StrongBox<bool> yielded = null)
        {
            return new JoinableTaskFactory.MainThreadAwaitable(Instance.SwitchToUIThread(yielded));
        }

        //public void ExecuteSynchronously(Func<Task> asyncAction)
        //{
        //    throw new NotImplementedException();
        //}

        //public T ExecuteSynchronously<T>(Func<Task<T>> asyncAction)
        //{
        //    throw new NotImplementedException();
        //}

        //public void VerifyOnUIThread()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Fork(Func<Task> asyncAction, JoinableTaskFactory factory = null, global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject unconfiguredProject = null, global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject configuredProject = null, ErrorReportSettings watsonReportSettings = null, ProjectFaultSeverity faultSeverity = ProjectFaultSeverity.Recoverable, ForkOptions options = ForkOptions.Default)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
