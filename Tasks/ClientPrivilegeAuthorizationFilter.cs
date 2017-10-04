using Hangfire.Annotations;
using Hangfire.Dashboard;
using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using Microsoft.Owin;

namespace Tasks
{
    public class ClientPrivilegeAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var env = context.GetOwinEnvironment();
            var owinContext = new OwinContext(env);

            if (owinContext.Authentication.User.Identity.IsAuthenticated)
            {
                if (CacheManager.Current.CurrentUser.HasPriv(ClientPrivilege.Developer))
                {
                    return true;
                }
            }

            return false;
        }
    }
}