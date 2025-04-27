using System.Web;
using System.Web.Optimization;
using System.Web.UI.WebControls;

namespace BanSach
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                        "~/Content/js/DataTables.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));
            bundles.Add(new StyleBundle("~/Content/css/TopBanChay").Include(
         "~/Content/css/TopBanChay.css"));
            bundles.Add(new StyleBundle("~/Content/css/trangsp.css").Include(
         "~/Content/css/trangsp.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/site.css"));

            //Admin
            bundles.Add(new StyleBundle("~/Admin/css").Include(
                      "~/Admin/css/LayOutAdmin.css"));
            bundles.Add(new ScriptBundle("~/Admin/js").Include(
                      "~/Admin/js/LayOutAdmin.js"));

            BundleTable.EnableOptimizations = true; // Bật tối ưu hóa
        }
    }
}
