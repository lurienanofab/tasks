using Hangfire;
using LNF.Impl;
using LNF.WebApi;
using Microsoft.Owin;
using Owin;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

[assembly: OwinStartup(typeof(Tasks.Startup))]

namespace Tasks
{
    public class Startup : OwinStartup
    {
        public override void Configuration(IAppBuilder app)
        {
            // route and data access setup
            base.Configuration(app);

            // webapi setup
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            app.UseWebApi(config);

            // hangfire setup
            var hangfireOpts = new DashboardOptions()
            {
                Authorization = new[] { new ClientPrivilegeAuthorizationFilter() },
                AppPath = VirtualPathUtility.ToAbsolute("~")
            };

            app.UseHangfireDashboard("/hangfire", hangfireOpts);
        }

        public override void ConfigureRoutes(RouteCollection routes)
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(routes);
        }
    }
}