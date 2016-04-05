// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.Shell.Interop
{
    public enum __VSHPROPID
    {
        VSHPROPID_ExtObject = -2027,
    }

    [InnerType(typeof(global::Microsoft.VisualStudio.Shell.Interop.IVsHierarchy))]
    public interface IVsHierarchy : IVsAdapter
    {
        //int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] out uint pdwCookie);
        //int Close();
        //int GetCanonicalName([ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] uint itemid, out string pbstrName);
        //int GetGuidProperty([ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] uint itemid, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSHPROPID")] int propid, out Guid pguid);
        //int GetNestedHierarchy([ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] uint itemid, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.REFIID")] ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] out uint pitemidNested);
        int GetProperty(uint itemid, int propid, out object pvar);
        //int GetSite(out OLE.Interop.IServiceProvider ppSP);
        //int ParseCanonicalName([ComAliasName("Microsoft.VisualStudio.OLE.Interop.LPCOLESTR")] string pszName, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] out uint pitemid);
        //int QueryClose([ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] out int pfCanClose);
        //int SetGuidProperty([ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] uint itemid, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSHPROPID")] int propid, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.REFGUID")] ref Guid rguid);
        //int SetProperty([ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] uint itemid, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSHPROPID")] int propid, object var);
        //int SetSite(OLE.Interop.IServiceProvider psp);
        //int UnadviseHierarchyEvents([ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] uint dwCookie);
        //int Unused0();
        //int Unused1();
        //int Unused2();
        //int Unused3();
        //int Unused4();
    }
}
