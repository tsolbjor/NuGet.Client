// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using NuGet.VisualStudio.Adapters;

namespace Adapters.Microsoft.VisualStudio.Shell.Interop
{
    [InnerType(typeof(global::Microsoft.VisualStudio.Shell.Interop.IVsProject))]
    public class VsProjectAdapter : VsAdapterBase, IVsProject
    {
        internal VsProjectAdapter() { }

        public VsProjectAdapter(global::Microsoft.VisualStudio.Shell.Interop.IVsProject vsProject)
            : base(vsProject)
        { }

        private global::Microsoft.VisualStudio.Shell.Interop.IVsProject Instance
        {
            get { return (global::Microsoft.VisualStudio.Shell.Interop.IVsProject)_instance; }
        }

        #region PassThroughs
        //public int AddItem(uint itemidLoc, VSADDITEMOPERATION dwAddItemOperation, string pszItemName, uint cFilesToOpen, string[] rgpszFilesToOpen, IntPtr hwndDlgOwner, VSADDRESULT[] pResult)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GenerateUniqueItemName(uint itemidLoc, string pszExt, string pszSuggestedRoot, out string pbstrItemName)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetItemContext(uint itemid, out global::Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetMkDocument(uint itemid, out string pbstrMkDocument)
        //{
        //    throw new NotImplementedException();
        //}

        //public int IsDocumentInProject(string pszMkDocument, out int pfFound, VSDOCUMENTPRIORITY[] pdwPriority, out uint pitemid)
        //{
        //    throw new NotImplementedException();
        //}

        //public int OpenItem(uint itemid, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
