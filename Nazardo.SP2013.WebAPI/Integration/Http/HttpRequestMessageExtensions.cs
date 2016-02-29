using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Nazardo.SP2013.WebAPI.Integration.Http
{
    internal static class HttpRequestMessageExtensions
    {
        internal static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string message, string messageDetail)
        {
            return request.CreateErrorResponse(statusCode, (bool includeErrorDetail) =>
            {
                if (!includeErrorDetail)
                {
                    return new HttpError(message);
                }
                var error = new HttpError(message);
                error.Add(HttpErrorKeys.MessageDetailKey, messageDetail);
                return error;
            });
        }

        private static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, Func<bool, HttpError> errorCreator)
        {
            HttpResponseMessage httpResponseMessage;
            HttpConfiguration configuration = request.GetConfiguration();
            HttpError httpError = errorCreator(request.ShouldIncludeErrorDetail());
            if (configuration != null)
            {
                return request.CreateResponse<HttpError>(statusCode, httpError, configuration);
            }
            using (HttpConfiguration httpConfiguration = new HttpConfiguration())
            {
                httpResponseMessage = request.CreateResponse<HttpError>(statusCode, httpError, httpConfiguration);
            }
            return httpResponseMessage;
        }
    }
}
