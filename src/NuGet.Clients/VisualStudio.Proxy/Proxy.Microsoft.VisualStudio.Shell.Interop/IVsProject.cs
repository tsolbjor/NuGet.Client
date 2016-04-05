// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Proxy;

namespace Proxy.Microsoft.VisualStudio.Shell.Interop
{
    [InnerType(typeof(global::Microsoft.VisualStudio.Shell.Interop.IVsProject))]
    public interface IVsProject : IVsProxy
    {
        //int AddItem(uint itemidLoc, VSADDITEMOPERATION dwAddItemOperation, string pszItemName, uint cFilesToOpen, string[] rgpszFilesToOpen, IntPtr hwndDlgOwner, VSADDRESULT[] pResult);
        //int GenerateUniqueItemName(uint itemidLoc, string pszExt, string pszSuggestedRoot, out string pbstrItemName);
        //int GetItemContext(uint itemid, out global::Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP);
        //int GetMkDocument(uint itemid, out string pbstrMkDocument);
        //int IsDocumentInProject(string pszMkDocument, out int pfFound, VSDOCUMENTPRIORITY[] pdwPriority, out uint pitemid);
        //int OpenItem(uint itemid, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame);
    }
}
