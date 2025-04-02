using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSach.DesignPatterns.TemplateMethodPattern
{
    public abstract class LogoutHandler
    {
        protected ControllerContext Context;

        protected LogoutHandler(ControllerContext context)
        {
            Context = context;
        }

        public ActionResult Logout()
        {
            ClearSession();
            return Redirect();
        }

        protected abstract void ClearSession();
        protected abstract ActionResult Redirect();
    }
}