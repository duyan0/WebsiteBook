using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;

namespace BanSach
{
    public class RouteConfig
    {

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "SanPhams", action = "TrangChu", id = UrlParameter.Optional }
            );
           
            routes.MapRoute(
                name: "DaNhanHang",
                url: "DonHang/DaNhanHang/{id}",
                defaults: new { controller = "DonHang", action = "DaNhanHang", id = UrlParameter.Optional }
            );

            // Thêm route cho trang bảo trì
            routes.MapRoute(
                name: "Maintenance",
                url: "Home/Maintenance",
                defaults: new { controller = "Home", action = "Maintenance" }
            );
        }

    }
}
