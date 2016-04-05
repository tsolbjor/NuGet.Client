// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.ProjectService))]
    public class ProjectServiceAdapter : VsAdapterBase, ProjectService
    {
        internal ProjectServiceAdapter() { }

        public ProjectServiceAdapter(global::Microsoft.VisualStudio.ProjectSystem.ProjectService projectService) :
            base(projectService)
        {
        }

        private global::Microsoft.VisualStudio.ProjectSystem.ProjectService Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.ProjectService)_instance; }
        }

        #region PassThroughs
        //public IEnumerable<ProjectSystem.UnconfiguredProject> LoadedUnconfiguredProjects
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public IProjectServices Services
        {
            get { return new ProjectServicesAdapter(Instance.Services); }
        }

        //public IImmutableSet<string> ServiceCapabilities
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

        //public event EventHandler Changed;

        //public Task<ProjectSystem.UnconfiguredProject> LoadProjectAsync(string projectLocation, IImmutableSet<string> projectCapabilities = null)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<ProjectSystem.UnconfiguredProject> LoadProjectAsync(System.Xml.XmlReader reader, IImmutableSet<string> projectCapabilities = null)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UnloadProjectAsync(ProjectSystem.UnconfiguredProject project)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsProjectCapabilityPresent(string projectCapability)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
