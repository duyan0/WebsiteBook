using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.TemplateMethodPattern
{
    public class AdminLogoutHandler : LogoutHandler
    {
        public AdminLogoutHandler(ControllerContext context) : base(context) { }

        protected override void ClearSession()
        {
            if (Context.HttpContext.Session["IdQly"] != null)
            {
                Context.HttpContext.Session.Clear();
            }
        }

        protected override ActionResult Redirect()
        {
            return new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
            {
                { "action", "Login" },
                { "controller", "LoginUSer" }
            });
        }
    }
}