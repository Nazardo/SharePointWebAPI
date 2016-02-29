using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Nazardo.SP2013.WebAPI.Controllers
{
    /// <summary>
    /// Simple test controller to validate WebApi integration
    /// </summary>
    public sealed class HelloController : ApiController
    {
        private readonly string[] _names = { "Alice", "Bob", "Mike", "Tom" };

        /// <summary>
        /// Returns a standard hello message.
        /// This request method is bound by convention to the request:
        /// HTTP GET /api/hello
        /// </summary>
        public string Get()
        {
            return "Hello World";
        }

        /// <summary>
        /// Returns a specific hello message.
        /// This request method is bound by convention to the request:
        /// HTTP GET /api/hello/{id}
        /// </summary>
        public string Get(int id)
        {
            if (id >= 0 && id < _names.Length)
            {
                return string.Format("Hello {0} (id: {1})", _names[id], id);
            }
            return string.Format("Hello unknown user with id {0}", id);
        }
    }
}
