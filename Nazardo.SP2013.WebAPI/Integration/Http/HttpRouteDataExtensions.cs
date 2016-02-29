using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Nazardo.SP2013.WebAPI.Integration.Http
{
    internal static class HttpRouteDataExtensions
    {
        internal static CandidateAction[] GetDirectRouteCandidates(this IHttpRouteData routeData)
        {
            IEnumerable<IHttpRouteData> subRoutes = routeData.GetSubRoutes();
            if (subRoutes == null)
            {
                if (routeData.Route == null)
                {
                    return null;
                }
                return routeData.Route.GetDirectRouteCandidates();
            }
            List<CandidateAction> candidateActions = new List<CandidateAction>();
            foreach (IHttpRouteData subRoute in subRoutes)
            {
                CandidateAction[] directRouteCandidates = subRoute.Route.GetDirectRouteCandidates();
                if (directRouteCandidates == null)
                {
                    continue;
                }
                candidateActions.AddRange(directRouteCandidates);
            }
            return candidateActions.ToArray();
        }

        public static IEnumerable<IHttpRouteData> GetSubRoutes(this IHttpRouteData routeData)
        {
            IHttpRouteData[] httpRouteDataArray = null;
            if (routeData.Values.TryGetValue<IHttpRouteData[]>("MS_SubRoutes", out httpRouteDataArray))
            {
                return httpRouteDataArray;
            }
            return null;
        }

        public static void RemoveOptionalRoutingParameters(this IHttpRouteData routeData)
        {
            HttpRouteDataExtensions.RemoveOptionalRoutingParameters(routeData.Values);
            IEnumerable<IHttpRouteData> subRoutes = routeData.GetSubRoutes();
            if (subRoutes != null)
            {
                foreach (IHttpRouteData subRoute in subRoutes)
                {
                    subRoute.RemoveOptionalRoutingParameters();
                }
            }
        }

        private static void RemoveOptionalRoutingParameters(IDictionary<string, object> routeValueDictionary)
        {
            int count = routeValueDictionary.Count;
            int num = 0;
            string[] key = new string[count];
            foreach (KeyValuePair<string, object> keyValuePair in routeValueDictionary)
            {
                if (keyValuePair.Value != RouteParameter.Optional)
                {
                    continue;
                }
                key[num] = keyValuePair.Key;
                num++;
            }
            for (int i = 0; i < num; i++)
            {
                routeValueDictionary.Remove(key[i]);
            }
        }
    }

}
