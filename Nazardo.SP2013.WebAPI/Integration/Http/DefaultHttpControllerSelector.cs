using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace Nazardo.SP2013.WebAPI.Integration.Http
{
    internal class DefaultHttpControllerSelector : IHttpControllerSelector
    {
        private const string ControllerKey = "controller";

        public readonly static string ControllerSuffix;

        private readonly HttpConfiguration _configuration;

        private readonly HttpControllerTypeCache _controllerTypeCache;

        private readonly Lazy<ConcurrentDictionary<string, HttpControllerDescriptor>> _controllerInfoCache;

        static DefaultHttpControllerSelector()
        {
            DefaultHttpControllerSelector.ControllerSuffix = "Controller";
        }

        public DefaultHttpControllerSelector(HttpConfiguration configuration)
        {
            if (configuration == null)
            {
                throw Error.ArgumentNull("configuration");
            }
            this._controllerInfoCache = new Lazy<ConcurrentDictionary<string, HttpControllerDescriptor>>(new Func<ConcurrentDictionary<string, HttpControllerDescriptor>>(this.InitializeControllerInfoCache));
            this._configuration = configuration;
            this._controllerTypeCache = new HttpControllerTypeCache(this._configuration);
        }

        private static Exception CreateAmbiguousControllerException(IHttpRoute route, string controllerName, ICollection<Type> matchingTypes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Type matchingType in matchingTypes)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(matchingType.FullName);
            }
            string defaultControllerFactoryControllerNameAmbiguousWithRouteTemplate = "DefaultControllerFactory_ControllerNameAmbiguous_WithRouteTemplate {0} {1} {2} {3}";
            object[] objArray = new object[] { controllerName, route.RouteTemplate, stringBuilder, Environment.NewLine };
            return new InvalidOperationException(Error.Format(defaultControllerFactoryControllerNameAmbiguousWithRouteTemplate, objArray));
        }

        private static Exception CreateDirectRouteAmbiguousControllerException(CandidateAction[] candidates)
        {
            HashSet<Type> types = new HashSet<Type>();
            for (int i = 0; i < (int)candidates.Length; i++)
            {
                types.Add(candidates[i].ActionDescriptor.ControllerDescriptor.ControllerType);
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Type type in types)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(type.FullName);
            }
            string directRouteAmbiguousController = "DirectRoute_AmbiguousController {0} {1}";
            object[] newLine = new object[] { stringBuilder, Environment.NewLine };
            return Error.InvalidOperation(directRouteAmbiguousController, newLine);
        }

        public virtual IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return this._controllerInfoCache.Value.ToDictionary<KeyValuePair<string, HttpControllerDescriptor>, string, HttpControllerDescriptor>((KeyValuePair<string, HttpControllerDescriptor> c) => c.Key, (KeyValuePair<string, HttpControllerDescriptor> c) => c.Value, StringComparer.OrdinalIgnoreCase);
        }

        public virtual string GetControllerName(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw Error.ArgumentNull("request");
            }
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                return null;
            }
            string str = null;
            routeData.Values.TryGetValue<string>("controller", out str);
            return str;
        }

        private static HttpControllerDescriptor GetDirectRouteController(IHttpRouteData routeData)
        {
            CandidateAction[] directRouteCandidates = routeData.GetDirectRouteCandidates();
            if (directRouteCandidates == null)
            {
                return null;
            }
            HttpControllerDescriptor controllerDescriptor = directRouteCandidates[0].ActionDescriptor.ControllerDescriptor;
            for (int i = 1; i < (int)directRouteCandidates.Length; i++)
            {
                if (directRouteCandidates[i].ActionDescriptor.ControllerDescriptor != controllerDescriptor)
                {
                    throw DefaultHttpControllerSelector.CreateDirectRouteAmbiguousControllerException(directRouteCandidates);
                }
            }
            return controllerDescriptor;
        }

        private ConcurrentDictionary<string, HttpControllerDescriptor> InitializeControllerInfoCache()
        {
            HttpControllerDescriptor httpControllerDescriptor;
            ConcurrentDictionary<string, HttpControllerDescriptor> strs = new ConcurrentDictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> strs1 = new HashSet<string>();
            foreach (KeyValuePair<string, ILookup<string, Type>> cache in this._controllerTypeCache.Cache)
            {
                string key = cache.Key;
                foreach (IGrouping<string, Type> value in cache.Value)
                {
                    foreach (Type type in value)
                    {
                        if (!strs.Keys.Contains(key))
                        {
                            strs.TryAdd(key, new HttpControllerDescriptor(this._configuration, key, type));
                        }
                        else
                        {
                            strs1.Add(key);
                            break;
                        }
                    }
                }
            }
            foreach (string str in strs1)
            {
                strs.TryRemove(str, out httpControllerDescriptor);
            }
            return strs;
        }

        public virtual HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            HttpControllerDescriptor directRouteController;
            if (request == null)
            {
                throw Error.ArgumentNull("request");
            }
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData != null)
            {
                directRouteController = DefaultHttpControllerSelector.GetDirectRouteController(routeData);
                if (directRouteController != null)
                {
                    return directRouteController;
                }
            }
            string controllerName = this.GetControllerName(request);
            if (string.IsNullOrEmpty(controllerName))
            {
                string resourceNotFound = "ResourceNotFound {0}";
                object[] requestUri = new object[] { request.RequestUri };
                string str = Error.Format(resourceNotFound, requestUri);
                string controllerNameNotFound = "ControllerNameNotFound {1}";
                object[] objArray = new object[] { request.RequestUri };
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, str, Error.Format(controllerNameNotFound, objArray)));
            }
            if (!this._controllerInfoCache.Value.TryGetValue(controllerName, out directRouteController))
            {
                ICollection<Type> controllerTypes = this._controllerTypeCache.GetControllerTypes(controllerName);
                if (controllerTypes.Count != 0)
                {
                    throw DefaultHttpControllerSelector.CreateAmbiguousControllerException(request.GetRouteData().Route, controllerName, controllerTypes);
                }
                string resourceNotFound1 = "ResourceNotFound {0}";
                object[] requestUri1 = new object[] { request.RequestUri };
                string str1 = Error.Format(resourceNotFound1, requestUri1);
                string defaultControllerFactoryControllerNameNotFound = "DefaultControllerFactory_ControllerNameNotFound {0}";
                object[] objArray1 = new object[] { controllerName };
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, str1, Error.Format(defaultControllerFactoryControllerNameNotFound, objArray1)));
            }
            return directRouteController;
        }
    }

}
