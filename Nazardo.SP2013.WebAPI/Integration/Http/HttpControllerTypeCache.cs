using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Nazardo.SP2013.WebAPI.Integration.Http
{
    internal sealed class HttpControllerTypeCache
    {
        private readonly HttpConfiguration _configuration;

        private readonly Lazy<Dictionary<string, ILookup<string, Type>>> _cache;

        internal Dictionary<string, ILookup<string, Type>> Cache
        {
            get
            {
                return this._cache.Value;
            }
        }

        public HttpControllerTypeCache(HttpConfiguration configuration)
        {
            if (configuration == null)
            {
                throw Error.ArgumentNull("configuration");
            }
            this._configuration = configuration;
            this._cache = new Lazy<Dictionary<string, ILookup<string, Type>>>(new Func<Dictionary<string, ILookup<string, Type>>>(this.InitializeCache));
        }

        public ICollection<Type> GetControllerTypes(string controllerName)
        {
            ILookup<string, Type> strs;
            if (string.IsNullOrEmpty(controllerName))
            {
                throw Error.ArgumentNullOrEmpty("controllerName");
            }
            HashSet<Type> types = new HashSet<Type>();
            if (this._cache.Value.TryGetValue(controllerName, out strs))
            {
                foreach (IGrouping<string, Type> strs1 in strs)
                {
                    types.UnionWith(strs1);
                }
            }
            return types;
        }

        private Dictionary<string, ILookup<string, Type>> InitializeCache()
        {
            IAssembliesResolver assembliesResolver = this._configuration.Services.GetAssembliesResolver();
            IHttpControllerTypeResolver httpControllerTypeResolver = this._configuration.Services.GetHttpControllerTypeResolver();
            IEnumerable<IGrouping<string, Type>> groupings = httpControllerTypeResolver.GetControllerTypes(assembliesResolver).GroupBy<Type, string>((Type t) => t.Name.Substring(0, t.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length), StringComparer.OrdinalIgnoreCase);
            return groupings.ToDictionary<IGrouping<string, Type>, string, ILookup<string, Type>>((IGrouping<string, Type> g) => g.Key, (IGrouping<string, Type> g) => g.ToLookup<Type, string>((Type t) => t.Namespace ?? string.Empty, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
        }
    }

}
