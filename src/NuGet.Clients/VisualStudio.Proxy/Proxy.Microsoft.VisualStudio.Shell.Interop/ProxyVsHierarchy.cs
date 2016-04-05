// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using NuGet.VisualStudio.Proxy;

namespace Proxy.Microsoft.VisualStudio.Shell.Interop
{
    [InnerType(typeof(global::Microsoft.VisualStudio.Shell.Interop.IVsHierarchy))]
    public class ProxyVsHierarchy : VsProxy, IVsHierarchy
    {
        internal ProxyVsHierarchy() { }

        public ProxyVsHierarchy(global::Microsoft.VisualStudio.Shell.Interop.IVsHierarchy vsHierarchy)
            : base(vsHierarchy)
        {
        }

        private global::Microsoft.VisualStudio.Shell.Interop.IVsHierarchy Instance
        {
            get { return (global::Microsoft.VisualStudio.Shell.Interop.IVsHierarchy)_instance; }
        }

        #region PassThroughs
        //public int SetSite(OLE.Interop.IServiceProvider psp)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetSite(out OLE.Interop.IServiceProvider ppSP)
        //{
        //    throw new NotImplementedException();
        //}

        //public int QueryClose(out int pfCanClose)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Close()
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetGuidProperty(uint itemid, int propid, out Guid pguid)
        //{
        //    throw new NotImplementedException();
        //}

        //public int SetGuidProperty(uint itemid, int propid, ref Guid rguid)
        //{
        //    throw new NotImplementedException();
        //}

        public int GetProperty(uint itemid, int propid, out object pvar)
        {
            return Instance.GetProperty(itemid, propid, out pvar);
        }

        //public int SetProperty(uint itemid, int propid, object var)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetCanonicalName(uint itemid, out string pbstrName)
        //{
        //    throw new NotImplementedException();
        //}

        //public int ParseCanonicalName(string pszName, out uint pitemid)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Unused0()
        //{
        //    throw new NotImplementedException();
        //}

        //public int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
        //{
        //    throw new NotImplementedException();
        //}

        //public int UnadviseHierarchyEvents(uint dwCookie)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Unused1()
        //{
        //    throw new NotImplementedException();
        //}

        //public int Unused2()
        //{
        //    throw new NotImplementedException();
        //}

        //public int Unused3()
        //{
        //    throw new NotImplementedException();
        //}

        //public int Unused4()
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
