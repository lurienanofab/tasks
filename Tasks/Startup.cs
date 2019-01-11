using Hangfire;
using LNF;
using LNF.Web;
using LNF.WebApi;
using Microsoft.Owin;
using Owin;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using LNF.Impl.DependencyInjection.Web;

[assembly: OwinStartup(typeof(Tasks.Startup))]

namespace Tasks
{
    public class Startup : OwinStartup
    {
        public override void Configuration(IAppBuilder app)
        {
            ServiceProvider.Current = IOC.Resolver.GetInstance<ServiceProvider>();

            // route and data access setup
            base.Configuration(app);

            // webapi setup
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            HangfireBootstrapper.Instance.Start();

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