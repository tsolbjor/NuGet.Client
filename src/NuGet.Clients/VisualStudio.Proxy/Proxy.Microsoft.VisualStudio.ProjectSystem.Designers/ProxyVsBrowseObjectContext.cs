// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using NuGet.VisualStudio.Proxy;
using Microsoft.VisualStudio.ProjectSystem.Properties;

namespace Proxy.Microsoft.VisualStudio.ProjectSystem.Designers
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext))]
    public class ProxyVsBrowseObjectContext : VsProxy, IVsBrowseObjectContext
    {
        internal ProxyVsBrowseObjectContext() { }

        public ProxyVsBrowseObjectContext(global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext vsBrowseObjectContext) :
            base(vsBrowseObjectContext)
        { }

        internal global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext)_instance; }
        }


        #region PassThroughs
        public ConfiguredProject ConfiguredProject
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IProjectPropertiesContext ProjectPropertiesContext
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IPropertySheet PropertySheet
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public UnconfiguredProject UnconfiguredProject
        {
            get
            {
                return new ProxyUnconfiguredProject(Instance.UnconfiguredProject);
            }
        }
        #endregion
    }
}
