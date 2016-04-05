// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Threading;
using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject))]
    public class ConfiguredProjectAdapter : VsAdapterBase, ConfiguredProject
    {
        public ConfiguredProjectAdapter() { }

        public ConfiguredProjectAdapter(global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject configuredProject) :
            base(configuredProject)
        { }

        internal global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject)_instance; }
        }

        #region PassThroughs
        //public System.Collections.Immutable.IImmutableSet<string> Capabilities
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public ProjectConfiguration ProjectConfiguration
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public IComparable ProjectVersion
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public System.Threading.Tasks.Dataflow.IReceivableSourceBlock<IComparable> ProjectVersionBlock
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public IConfiguredProjectServices Services
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public UnconfiguredProject UnconfiguredProject
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public IComparable Version
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        // TODO wrap these
        //public event EventHandler Changed;
        //public event EventHandler ProjectChanged;
        //public event EventHandler ProjectChangedSynchronous;
        //public event AsyncEventHandler ProjectUnloading;

        //public bool IsProjectCapabilityPresent(string projectCapability)
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        #endregion
    }
}
