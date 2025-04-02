
using BanSach.Models;
using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity.AspNet.Mvc;
using Unity.Lifetime;
using Unity;
using PayPal.Api;

namespace BanSach
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Khởi tạo cấu hình PayPal
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken);

        }

        // Kiểm tra chế độ bảo trì trong mỗi yêu cầu HTTP
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Kiểm tra chế độ bảo trì
            string maintenanceModeSetting = ConfigurationManager.AppSettings["IsMaintenanceMode"];
            bool isMaintenanceMode = false;//false and true

            if (!string.IsNullOrEmpty(maintenanceModeSetting))
            {
                bool.TryParse(maintenanceModeSetting, out isMaintenanceMode);
            }

            // Kiểm tra nếu đang ở chế độ bảo trì và không phải trang Maintenance
            if (isMaintenanceMode)
            {
                var currentPath = Request.Url.LocalPath.ToLower();

                // Nếu đang ở chế độ bảo trì và không phải trang Maintenance, chuyển hướng
                if (!currentPath.Contains("/home/index"))
                {
                    Response.Redirect("~/Home/Index", true); // Sử dụng true để dừng xử lý tiếp
                }
            }
        }
    }
}
