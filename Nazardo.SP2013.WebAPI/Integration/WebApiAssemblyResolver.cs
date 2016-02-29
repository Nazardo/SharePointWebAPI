using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace Nazardo.SP2013.WebAPI.Integration
{
    /// <summary>
    /// A custom resolver to find all assemblies with WebApi controllers.
    /// </summary>
    internal sealed class WebApiAssemblyResolver : IAssembliesResolver
    {
        public ICollection<Assembly> GetAssemblies()
        {
            return new List<Assembly> {
                // Add here all assemblies containing WebApi controllers
                // e.g. typeof(MyAssembly.ControllerClass).Assembly
                typeof(Controllers.HelloController).Assembly
            };
        }
    }
}