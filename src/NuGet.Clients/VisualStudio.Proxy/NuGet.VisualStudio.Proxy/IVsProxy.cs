// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace NuGet.VisualStudio.Proxy
{
    public interface IVsProxy
    {
        bool Is<T>() where T : class, IVsProxy;
        T As<T>() where T : class, IVsProxy;
        T Cast<T>() where T : class, IVsProxy;
    }
}
