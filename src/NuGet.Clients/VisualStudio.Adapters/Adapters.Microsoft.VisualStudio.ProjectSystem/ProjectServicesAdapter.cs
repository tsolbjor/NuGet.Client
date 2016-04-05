// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.ProjectSystem.Designers;
using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.IProjectServices))]
    public class ProjectServicesAdapter : VsAdapterBase, IProjectServices
    {
        internal ProjectServicesAdapter() { }

        public ProjectServicesAdapter(global::Microsoft.VisualStudio.ProjectSystem.IProjectServices projectServices) :
            base(projectServices)
        {
        }

        private global::Microsoft.VisualStudio.ProjectSystem.IProjectServices Instance
        {
            get
            {
                return (global::Microsoft.VisualStudio.ProjectSystem.IProjectServices)_instance;
            }
        }

        #region PassThroughs
        public IProjectLockService ProjectLockService
        {
            get
            {
                return new ProjectLockServiceAdapter(Instance.ProjectLockService);
            }
        }

        public IThreadHandling ThreadingPolicy
        {
            get
            {
                return new ThreadHandlingAdapter(Instance.ThreadingPolicy);
            }
        }

        //public IProjectFaultHandlerService FaultHandler
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public IProjectReloader ProjectReloader
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public VisualStudio.Composition.ExportProvider ExportProvider
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        #endregion
    }
}
