using LNF;
using LNF.Impl.DependencyInjection.Web;
using System;

namespace Tasks
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //ServiceProvider.Current = IOC.Resolver.GetInstance<ServiceProvider>();
            //HangfireBootstrapper.Instance.Start();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            HangfireBootstrapper.Instance.Stop();
        }
    }
}