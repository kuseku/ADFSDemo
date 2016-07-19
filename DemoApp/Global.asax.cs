using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DemoApp
{
    public class ADFSUser : System.Security.Principal.IPrincipal
    {
        public System.Security.Principal.IPrincipal User { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String BCMID { get; set; }
        public String ADUserName { get; set; }

        public String UPN { get; set; }

        public String FullName => $"{FirstName} {LastName}";

        IIdentity IPrincipal.Identity
        {
            get
            {
                return User.Identity;
            }
        }

        public ADFSUser(System.Security.Principal.IPrincipal user)
        {
            //Pass the Actual value as is
            User = user;

            var claim = User.Identity as ClaimsIdentity;
            FirstName = claim.Claims.Where(c => c.Type.EndsWith("givenname")).FirstOrDefault()?.Value;
            LastName = claim.Claims.Where(c => c.Type.EndsWith("surname")).FirstOrDefault()?.Value;
            UPN = claim.Claims.Where(c => c.Type.EndsWith("upn")).FirstOrDefault()?.Value;
            BCMID = claim.Claims.Where(c => c.Type.EndsWith("nameidentifier")).FirstOrDefault()?.Value;
            Email = claim.Claims.Where(c => c.Type.EndsWith("emailaddress")).FirstOrDefault()?.Value;
            ADUserName = claim.Claims.Where(c => c.Type.EndsWith("ImmutableID")).FirstOrDefault()?.Value;

            //claim.Claims.Where(c=>c.Type)
        }

        bool IPrincipal.IsInRole(string role)
        {
            return false;
        }
    }
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

      
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {

                
                HttpContext.Current.User = new ADFSUser(HttpContext.Current.User);
                


            }
        }
    }
}
