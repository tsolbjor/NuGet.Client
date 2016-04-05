// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace NuGet.VisualStudio.Adapters
{
    public class InnerTypeAttribute : Attribute
    {
        public InnerTypeAttribute(Type innerType)
        {
            InnerType = innerType;
        }

        public Type InnerType
        {
            get;
            private set;
        }
    }
}
