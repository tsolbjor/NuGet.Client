// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace NuGet.VisualStudio.Adapters
{
    public interface IVsAdapter
    {
        bool Is<T>() where T : class, IVsAdapter;
        T As<T>() where T : class, IVsAdapter;
        T Cast<T>() where T : class, IVsAdapter;
    }
}
