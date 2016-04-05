// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Proxy;

namespace Proxy.Microsoft.VisualStudio.ProjectSystem
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.ProjectService))]
    public interface ProjectService : IVsProxy
    {
        //IEnumerable<ProjectSystem.UnconfiguredProject> LoadedUnconfiguredProjects { get; }
        IProjectServices Services { get; }

        //IImmutableSet<string> ServiceCapabilities { get; }
        //IComparable Version { get; }
        //event EventHandler Changed;
        //Task<ProjectSystem.UnconfiguredProject> LoadProjectAsync(string projectLocation, IImmutableSet<string> projectCapabilities = null);
        //Task<ProjectSystem.UnconfiguredProject> LoadProjectAsync(System.Xml.XmlReader reader, IImmutableSet<string> projectCapabilities = null);
        //Task UnloadProjectAsync(ProjectSystem.UnconfiguredProject project);
        //bool IsProjectCapabilityPresent(string projectCapability);
    }
}
