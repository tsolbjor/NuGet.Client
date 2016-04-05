// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using NuGet.VisualStudio.Proxy;

namespace Proxy.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject))]
    public interface UnconfiguredProject : IVsProxy
    {
        //System.Collections.Immutable.IImmutableSet<string> Capabilities { get; }
        string FullPath { get; }
        //IEnumerable<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject> LoadedConfiguredProjects { get; }
        ProjectService ProjectService { get; }
        //bool RequiresReloadForExternalFileChange { get; }
        //IUnconfiguredProjectServices Services { get; }
        //IComparable Version { get; }

        //event EventHandler Changed;
        //event AsyncEventHandler<ProjectRenamedEventArgs> ProjectRenamed;
        //event AsyncEventHandler<ProjectRenamedEventArgs> ProjectRenamedOnWriter;
        //event AsyncEventHandler<ProjectRenamedEventArgs> ProjectRenaming;
        //event AsyncEventHandler ProjectUnloading;

        //Task<bool> CanRenameAsync(string newFilePath = null)
        //Task<Encoding> GetFileEncodingAsync();
        //Task<bool> GetIsDirtyAsync();
        Task<ConfiguredProject> GetSuggestedConfiguredProjectAsync();
        //bool IsProjectCapabilityPresent(string projectCapability);
        //public Task<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject> LoadConfiguredProjectAsync(ProjectConfiguration projectConfiguration);
        //Task<ConfiguredProject> LoadConfiguredProjectAsync(string name, System.Collections.Immutable.IImmutableDictionary<string, string> configurationProperties)
        //public Task RenameAsync(string newFilePath);
        //public Task SaveAsync(string filePath = null);
        //Task SaveCopyAsync(string filePath, Encoding fileEncoding = null);
        //Task SaveUserFileAsync();
        //Task SetFileEncodingAsync(Encoding value);
    }
}
