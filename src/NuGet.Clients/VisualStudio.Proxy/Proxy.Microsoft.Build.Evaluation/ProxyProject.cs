// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Proxy;

namespace Proxy.Microsoft.Build.Evaluation
{
    [InnerType(typeof(global::Microsoft.Build.Evaluation.Project))]
    public class ProxyProject : VsProxy
    {
        public ProxyProject() { }

        // TODO remove this--necessary for now to isolate the proxying to one class in the codebase.
        public global::Microsoft.Build.Evaluation.Project InnerInstance
        {
            get { return _instance as global::Microsoft.Build.Evaluation.Project; }
        }

        public ProxyProject(global::Microsoft.Build.Evaluation.Project instance) :
            base(instance)
        {
        }
    }
}
