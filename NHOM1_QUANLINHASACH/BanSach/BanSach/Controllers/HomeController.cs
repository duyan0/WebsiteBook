using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        // Action chính
        public ActionResult Index()
        {
            return View();
        }

        // Action bảo trì
        public ActionResult Maintenance()
        {
            return View();
        }
        public ActionResult Profile()
        {
            return View();
        }
        public ActionResult Forbidden()
        {
            // Bạn có thể hiển thị một trang lỗi tùy chỉnh ở đây
            return View();
        }
    }
}