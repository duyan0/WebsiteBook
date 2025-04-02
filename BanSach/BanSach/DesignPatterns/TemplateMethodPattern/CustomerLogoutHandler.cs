using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.TemplateMethodPattern
{
    public class CustomerLogoutHandler : LogoutHandler
    {
        public CustomerLogoutHandler(ControllerContext context) : base(context) { }

        protected override void ClearSession()
        {
            if (Context.HttpContext.Session["Idkh"] != null)
            {
                Context.HttpContext.Session["Idkh"] = null;
                Context.HttpContext.Session["IDkh"] = null;
                Context.HttpContext.Session["TenKH"] = null;
                Context.HttpContext.Session["SoDT"] = null;
            }
        }
        protected override ActionResult Redirect()
        {
            return new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
            {
                { "action", "LoginAccountCus" },
                { "controller", "LoginUser" }
            });
        }
    }
}