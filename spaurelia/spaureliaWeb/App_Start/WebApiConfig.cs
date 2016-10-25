using System.Web.Http;

namespace spaureliaWeb
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.SuppressDefaultHostAuthentication();
            //configuration.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.MapHttpAttributeRoutes();

            //var cors = new EnableCorsAttribute("https://dunite.sharepoint.com", "*", "*");
            //cors.SupportsCredentials = true;
            //config.EnableCors(cors);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}