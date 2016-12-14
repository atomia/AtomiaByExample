using System.Reflection;
using System.Web.Routing;
using Atomia.Web.Base.Interfaces;
using Atomia.Web.Plugin.HCP.Provisioning.Helpers;

namespace Atomia.Web.Plugin.Example.Extensions
{
    public class PartialsProvider : IPartialsProvider
    {
        /// <summary>
        /// Fetches the page partial items(widgets).
        /// </summary>
        /// <param name="controller">The plugin controller.</param>
        /// <param name="action">The plugin action(view)</param>
        /// <param name="routeData">The route data from the current executing context.</param>
        /// <returns>Object containing page partial items</returns>
        public object FetchPagePartialItems(string controller, string action, RouteData routeData)
        {
            return PartialItemsFetcher.FetchPagePartialItems(controller, action, routeData, "Atomia.Web.Plugin.Example.dll.config", Assembly.GetExecutingAssembly().CodeBase);
        }

        /// <summary>
        /// Fetches the plugin partial items(widgets).
        /// </summary>
        /// <param name="routeData">The route data from the current executing context.</param>
        /// <returns>Object containing plugin partial items</returns>
        public object FetchPluginPartialItems(RouteData routeData)
        {
            return null;
        }

        /// <summary>
        /// Fetches the partial items for the given page in the given placeholder(container).
        /// </summary>
        /// <param name="controller">The plugin controller.</param>
        /// <param name="action">The plugin action(view).</param>
        /// <param name="routeData">The route data from the current executing context.</param>
        /// <param name="container">The placeholder(container).</param>
        /// <returns>
        /// Object containing page partial items in the given placeholder (container)
        /// </returns>
        public object FetchPageContainerPartialItems(string controller, string action, RouteData routeData, string container)
        {
            return PartialItemsFetcher.FetchPageContainerPartialItems(controller, action, routeData, container, "Atomia.Web.Plugin.Example.dll.config", Assembly.GetExecutingAssembly().CodeBase);
        }

        /// <summary>
        /// Fetches the partial items for the given placeholder(container).
        /// </summary>
        /// <param name="routeData">The route data from the current executing context.</param>
        /// <param name="container">The placeholder(container).</param>
        /// <returns>
        /// Object containing plugin partial items for the given placeholder (container)
        /// </returns>
        public object FetchPluginContainerPartialItems(RouteData routeData, string container)
        {
            return null;
        }
    }
}