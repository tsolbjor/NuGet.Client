// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.ProjectSystem.Designers
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext))]
    public interface IVsBrowseObjectContext : IVsAdapter
    {
        //ConfiguredProject ConfiguredProject { get; }
        //IProjectPropertiesContext ProjectPropertiesContext { get; }
        //IPropertySheet PropertySheet { get; }
        UnconfiguredProject UnconfiguredProject { get; }
    }
}
