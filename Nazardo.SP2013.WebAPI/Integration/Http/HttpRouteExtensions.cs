using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Nazardo.SP2013.WebAPI.Integration.Http
{
    internal static class HttpRouteExtensions
    {
        public static CandidateAction[] GetDirectRouteCandidates(this IHttpRoute route)
        {
            HttpActionDescriptor[] httpActionDescriptorArray;
            int num;
            decimal num1;
            IDictionary<string, object> dataTokens = route.DataTokens;
            if (dataTokens == null)
            {
                return null;
            }
            List<CandidateAction> candidateActions = new List<CandidateAction>();
            HttpActionDescriptor[] httpActionDescriptorArray1 = null;
            if (dataTokens.TryGetValue<HttpActionDescriptor[]>("actions", out httpActionDescriptorArray) && httpActionDescriptorArray != null && (int)httpActionDescriptorArray.Length > 0)
            {
                httpActionDescriptorArray1 = httpActionDescriptorArray;
            }
            if (httpActionDescriptorArray1 == null)
            {
                return null;
            }
            int num2 = 0;
            if (dataTokens.TryGetValue<int>("order", out num))
            {
                num2 = num;
            }
            decimal num3 = new decimal(0);
            if (dataTokens.TryGetValue<decimal>("precedence", out num1))
            {
                num3 = num1;
            }
            HttpActionDescriptor[] httpActionDescriptorArray2 = httpActionDescriptorArray1;
            for (int i = 0; i < (int)httpActionDescriptorArray2.Length; i++)
            {
                HttpActionDescriptor httpActionDescriptor = httpActionDescriptorArray2[i];
                CandidateAction candidateAction = new CandidateAction()
                {
                    ActionDescriptor = httpActionDescriptor,
                    Order = num2,
                    Precedence = num3
                };
                candidateActions.Add(candidateAction);
            }
            return candidateActions.ToArray();
        }

        public static HttpActionDescriptor[] GetTargetActionDescriptors(this IHttpRoute route)
        {
            HttpActionDescriptor[] httpActionDescriptorArray;
            IDictionary<string, object> dataTokens = route.DataTokens;
            if (dataTokens == null)
            {
                return null;
            }
            if (!dataTokens.TryGetValue<HttpActionDescriptor[]>("actions", out httpActionDescriptorArray))
            {
                return null;
            }
            return httpActionDescriptorArray;
        }

        public static HttpControllerDescriptor GetTargetControllerDescriptor(this IHttpRoute route)
        {
            HttpControllerDescriptor httpControllerDescriptor;
            IDictionary<string, object> dataTokens = route.DataTokens;
            if (dataTokens == null)
            {
                return null;
            }
            if (!dataTokens.TryGetValue<HttpControllerDescriptor>("controller", out httpControllerDescriptor))
            {
                return null;
            }
            return httpControllerDescriptor;
        }
    }

}
