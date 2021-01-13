using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HelloGram
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings
                .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters
                .Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }

        protected void Application_BeginRequest()
        {
            // Solves the return back when logout problem
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError() as HttpException;
            Response.Clear();

            if (exception == null) return;

            // clear error on server
            Server.ClearError();

            switch (exception.GetHttpCode())
            {
                case 404:
                    // page not found
                    Response.Redirect("~/NotFound");
                    break;
                case 500:
                // server error
                default:
                    Response.Redirect($"~/Error?q={exception.Message}");
                    break;
            }
        }
    }
}
