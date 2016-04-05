// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;

namespace NuGet.VisualStudio.Adapters
{
    public abstract class VsAdapterBase : IVsAdapter
    {
        private Type _innerType;
        internal object _instance;

        internal VsAdapterBase() { }

        public VsAdapterBase(object instance)
        {
            Debug.Assert(instance != null, "A proxy instance must have an inner instance");
            _instance = instance;
        }

        private Type InnerType
        {
            get
            {
                if (_innerType == null)
                {
                    _innerType = VsAdapterBase.ProxyInnerType(this.GetType());
                }
                return _innerType;
            }
        }

        public bool Is<T>() where T : class, IVsAdapter
        {
            Type compareType = VsAdapterBase.ProxyInnerType(typeof(T));
            if (InnerType != null && compareType != null)
            {
                return InnerType.IsEquivalentTo(compareType) || InnerType.IsSubclassOf(compareType);
            }

            return false;
        }

        public T As<T>() where T : class, IVsAdapter
        {
            T instanceAsT = _instance as T;
            if (instanceAsT != null)
            {
                return (T)CreateVsProxy<T>(instanceAsT);
            }

            return default(T);
        }

        public T Cast<T>() where T: class, IVsAdapter
        {
            Type typeToCast = VsAdapterBase.ProxyInnerType(typeof(T));
            return (T)CreateVsProxy<T>(_instance);
        }

        /// <summary>
        /// Proxy factory method
        /// </summary>
        internal IVsAdapter CreateVsProxy<T>(object instance)
        {
            Debug.Assert(instance != null, "Proxy requires an inner instance");
            if (instance == null)
            {
                return null;
            }

            // TODO - fill out these cases

            // Note that instances are often interop COM types, which limits our options for type checking. GetType() returns System.__ComObject.
            if (typeof(T).IsEquivalentTo(typeof(global::Adapters.Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext)))
            {
                return new global::Adapters.Microsoft.VisualStudio.ProjectSystem.Designers.VsBrowseObjectContextAdapter(
                    instance as global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext);
            }

            if (typeof(T).IsEquivalentTo(typeof(global::Adapters.Microsoft.VisualStudio.Shell.Interop.IVsProject)))
            {
                return new global::Adapters.Microsoft.VisualStudio.Shell.Interop.VsProjectAdapter(
                    instance as global::Microsoft.VisualStudio.Shell.Interop.IVsProject);
            }

            return null;
        }

        // TODO: cache these
        private static Type ProxyInnerType(Type ProxyType)
        {
            Attribute[] attributes = System.Attribute.GetCustomAttributes(ProxyType);
            InnerTypeAttribute innerTypeAttribute = attributes?.FirstOrDefault(a => a is InnerTypeAttribute) as InnerTypeAttribute;
            return innerTypeAttribute?.InnerType;
        }
    }
}
