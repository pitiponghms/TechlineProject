using CRMAPI.Configuration;
using System.Web.Http;

namespace CrmAPI2
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(APIConfig.Register);
        }
    }
}