using System.Web.Http;

namespace CRMAPI.Configuration
{
    public class APIConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            // New code


            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}