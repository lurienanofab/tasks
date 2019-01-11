using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Owin;
using System.Linq;
using System.Security.Principal;
using System.Web.Security;

namespace Tasks
{
    public class ClientPrivilegeAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var ctx = new OwinContext(context.GetOwinEnvironment());
            var result = IsAuthorized(ctx.Authentication.User.Identity);
            return result;
        }

        public bool IsAuthorized(IIdentity ident)
        {
            if (ident.IsAuthenticated)
            {
                if (ident is FormsIdentity formsIdentity)
                {
                    var ticket = formsIdentity.Ticket;
                    var roles = ticket.UserData.Split('|');
                    return roles.Contains("Developer");
                }
            }

            return false;
        }
    }
}