// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Proxy;

namespace Proxy.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject))]
    public interface ConfiguredProject : IVsProxy
    {
        //System.Collections.Immutable.IImmutableSet<string> Capabilities { get; }
        //ProjectConfiguration ProjectConfiguration { get; }
        //IComparable ProjectVersion { get; }
        //System.Threading.Tasks.Dataflow.IReceivableSourceBlock<IComparable> ProjectVersionBlock { get; }
        //IConfiguredProjectServices Services { get; }
        //UnconfiguredProject UnconfiguredProject { get; }
        //IComparable Version { get; }

        //public event EventHandler Changed;
        //public event EventHandler ProjectChanged;
        //public event EventHandler ProjectChangedSynchronous;
        //public event AsyncEventHandler ProjectUnloading;

        //bool IsProjectCapabilityPresent(string projectCapability);
    }
}
