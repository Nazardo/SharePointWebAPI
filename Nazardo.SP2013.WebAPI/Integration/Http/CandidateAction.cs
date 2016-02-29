using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Nazardo.SP2013.WebAPI.Integration.Http
{
    [DebuggerDisplay("{DebuggerToString()}")]
    internal sealed class CandidateAction
    {
        public HttpActionDescriptor ActionDescriptor
        {
            get;
            set;
        }

        public int Order
        {
            get;
            set;
        }

        public decimal Precedence
        {
            get;
            set;
        }

        public CandidateAction()
        {
        }

        internal string DebuggerToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            object[] actionName = new object[] { this.ActionDescriptor.ActionName, this.Order, this.Precedence };
            return string.Format(currentCulture, "{0}, Order={1}, Prec={2}", actionName);
        }

        public bool MatchName(string actionName)
        {
            return string.Equals(this.ActionDescriptor.ActionName, actionName, StringComparison.OrdinalIgnoreCase);
        }

        public bool MatchVerb(HttpMethod method)
        {
            return this.ActionDescriptor.SupportedHttpMethods.Contains(method);
        }
    }
}
