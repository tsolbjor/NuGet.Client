// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.Build.Evaluation
{
    [InnerType(typeof(global::Microsoft.Build.Evaluation.Project))]
    public class ProjectAdapter : VsAdapterBase
    {
        public ProjectAdapter() { }

        // TODO remove this--necessary for now to isolate the proxying to one class in the codebase.
        public global::Microsoft.Build.Evaluation.Project InnerInstance
        {
            get { return _instance as global::Microsoft.Build.Evaluation.Project; }
        }

        public ProjectAdapter(global::Microsoft.Build.Evaluation.Project instance) :
            base(instance)
        {
        }
    }
}
