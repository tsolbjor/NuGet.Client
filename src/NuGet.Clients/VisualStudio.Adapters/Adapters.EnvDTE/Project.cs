// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Adapters;

namespace Adapters.EnvDTE
{
    [InnerType(typeof(EnvDTE.Project))]
    public interface Project : IVsAdapter
    {
        //CodeModel CodeModel { get; }
        //Projects Collection { get; }
        //ConfigurationManager ConfigurationManager { get; }
        //DTE DTE { get; }
        //string ExtenderCATID { get; }
        //dynamic ExtenderNames { get; }
        //string FileName { get; }
        //string FullName { get; }
        //Globals Globals { get; }
        //bool IsDirty { get; }
        //string Kind { get; }
        //string Name { get; }
        dynamic Object { get; }
        //ProjectItem ParentProjectItem { get; }
        //ProjectItems ProjectItems { get; }
        //Properties Properties { get; }
        //bool Saved { get; }
        //string UniqueName { get; }
        //void Delete();
        //dynamic get_Extender(string ExtenderName);
        void Save(string FileName = "");
        //void SaveAs(string NewFileName);
    }
}
