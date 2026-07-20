using System;
using System.Web;
using System.Web.Mvc;

namespace Gelinlik.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private readonly string _role;

        public AuthorizeRoleAttribute(string role)
        {
            _role = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userRole = httpContext.Session["UserRole"] as string;
            if (string.IsNullOrEmpty(userRole))
            {
                return false;
            }

            return userRole.Equals(_role, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Login/Index");
        }
    }
}
