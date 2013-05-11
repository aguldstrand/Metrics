using System.Web.Mvc;
using System.Web.Routing;

namespace Collector
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Init",
                url: "script",
                defaults: new { controller = "Collector", action = "Init", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Collect",
                url: "collect/{session}",
                defaults: new { controller = "Collector", action = "Collect", id = UrlParameter.Optional }
            );

            RouteTable.Routes.MapHubs();
        }
    }
}