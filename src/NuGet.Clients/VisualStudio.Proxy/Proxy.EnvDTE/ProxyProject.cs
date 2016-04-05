// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using NuGet.VisualStudio.Proxy;

namespace Proxy.EnvDTE
{
    [InnerType(typeof(global::EnvDTE.Project))]
    public class ProxyProject : VsProxy, Project
    {
        // TODO: wrap returned types in proxies

        internal ProxyProject() { }

        public ProxyProject(global::EnvDTE.Project project) :
            base(project)
        { }

        // TODO: move these to the base class?
        private global::EnvDTE.Project Instance
        {
            get { return (global::EnvDTE.Project)_instance; }
        }

        #region PassThroughs
        //public CodeModel CodeModel
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public Projects Collection
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public ConfigurationManager ConfigurationManager
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public DTE DTE
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public string ExtenderCATID
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public dynamic ExtenderNames
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public string FileName
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public string FullName
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public Globals Globals
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsDirty
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public string Kind
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public string Name
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public dynamic Object
        {
            get
            {
                return Instance.Object;
            }
        }

        //public ProjectItem ParentProjectItem
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public ProjectItems ProjectItems
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public Properties Properties
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool Saved
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public string UniqueName
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public void Delete()
        //{
        //    throw new NotImplementedException();
        //}

        //public dynamic get_Extender(string ExtenderName)
        //{
        //    throw new NotImplementedException();
        //}

        public void Save(string FileName = "")
        {
            Instance.Save(FileName);
        }

        //public void SaveAs(string NewFileName)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
