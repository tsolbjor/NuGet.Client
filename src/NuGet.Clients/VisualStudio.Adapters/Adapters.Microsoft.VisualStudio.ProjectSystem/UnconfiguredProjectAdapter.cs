// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject))]
    public class UnconfiguredProjectAdapter : VsAdapterBase, UnconfiguredProject
    {
        internal UnconfiguredProjectAdapter() { }

        public UnconfiguredProjectAdapter(global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject unconfiguredProject) :
            base(unconfiguredProject)
        { }

        private global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject)_instance; }
        }

        #region PassThroughs
        //public System.Collections.Immutable.IImmutableSet<string> Capabilities
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public string FullPath
        {
            get
            {
                return Instance.FullPath;
            }
        }

        //public IEnumerable<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject> LoadedConfiguredProjects
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public ProjectService ProjectService
        {
            get
            {
                return new ProjectServiceAdapter(Instance.ProjectService);
            }
        }

        //public bool RequiresReloadForExternalFileChange
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public IUnconfiguredProjectServices Services
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

        // TODO fire events on inner instance
        //public event EventHandler Changed;
        //public event AsyncEventHandler<ProjectRenamedEventArgs> ProjectRenamed;
        //public event AsyncEventHandler<ProjectRenamedEventArgs> ProjectRenamedOnWriter;
        //public event AsyncEventHandler<ProjectRenamedEventArgs> ProjectRenaming;
        //public event AsyncEventHandler ProjectUnloading;

        //public Task<bool> CanRenameAsync(string newFilePath = null)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Encoding> GetFileEncodingAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> GetIsDirtyAsync()
        //{
        //    throw new NotImplementedException();
        //}

        public Task<ConfiguredProject> GetSuggestedConfiguredProjectAsync()
        {
            var tcs = new TaskCompletionSource<ConfiguredProject>();
            Instance.GetSuggestedConfiguredProjectAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerException);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(new ConfiguredProjectAdapter(t.Result));
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        //public bool IsProjectCapabilityPresent(string projectCapability)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject> LoadConfiguredProjectAsync(ProjectConfiguration projectConfiguration)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject> LoadConfiguredProjectAsync(string name, System.Collections.Immutable.IImmutableDictionary<string, string> configurationProperties)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task RenameAsync(string newFilePath)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SaveAsync(string filePath = null)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SaveCopyAsync(string filePath, Encoding fileEncoding = null)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SaveUserFileAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SetFileEncodingAsync(Encoding value)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
