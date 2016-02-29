using System;
using System.Collections;
using System.Web.Caching;
using System.Web.Hosting;

namespace Nazardo.SP2013.WebAPI.Integration
{
    /// <summary>
    /// Virtual path provider that handles special cases.
    /// </summary>
    internal sealed class WebAPIVirtualPathProvider : VirtualPathProvider
    {
        public override string CombineVirtualPaths(string basePath, string relativePath)
        {
            return Previous.CombineVirtualPaths(basePath, relativePath);
        }

        public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
        {
            return Previous.CreateObjRef(requestedType);
        }

        /// <summary>
        /// This is the only method where we need to do something.
        /// For every request containing the tilde character (~)
        /// which is not targeting .axd files, we remove the evil character.
        /// </summary>
        public override bool DirectoryExists(string virtualDir)
        {
            //removing the evil character - otherwise the hell freezes and yeah, SharePoint.
            if (virtualDir != null &&
                virtualDir.StartsWith("~/") == true &&
                virtualDir.Contains(".axd") == false)
            {
                string tmp = virtualDir.TrimStart('~');
                return Previous.DirectoryExists(tmp);
            }
            try
            {
                return Previous.DirectoryExists(virtualDir);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool FileExists(string virtualPath)
        {
            return Previous.FileExists(virtualPath);
        }

        public override CacheDependency GetCacheDependency(
            string virtualPath,
            IEnumerable virtualPathDependencies,
            DateTime utcStart)
        {
            return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override string GetCacheKey(string virtualPath)
        {
            return Previous.GetCacheKey(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return Previous.GetDirectory(virtualDir);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return Previous.GetFile(virtualPath);
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            return Previous.GetFileHash(virtualPath, virtualPathDependencies);
        }
    }
}