// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.CompilerServices;
using Adapters.Microsoft.VisualStudio.Threading;
using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.IThreadHandling))]
    public interface IThreadHandling : IVsAdapter
    {
        //JoinableTaskFactory AsyncPump { get; }
        //bool IsOnMainThread { get; }
        //JoinableTaskContextNode JoinableTaskContext { get; }

        //void ExecuteSynchronously(Func<Task> asyncAction);
        //T ExecuteSynchronously<T>(Func<Task<T>> asyncAction);
        //void Fork(Func<Task> asyncAction, JoinableTaskFactory factory = null, UnconfiguredProject unconfiguredProject = null, ConfiguredProject configuredProject = null, ErrorReportSettings watsonReportSettings = null, ProjectFaultSeverity faultSeverity = ProjectFaultSeverity.Recoverable, ForkOptions options = ForkOptions.Default);
        JoinableTaskFactory.MainThreadAwaitable SwitchToUIThread(StrongBox<bool> yielded = null);
        //void VerifyOnUIThread();
    }
}
