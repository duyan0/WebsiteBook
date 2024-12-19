using BanSach.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class AdminsController : Controller
    {
        private readonly db_book1 db = new db_book1();
        public ActionResult Index(int? page, string searchString)
        {
            int pageSize = 10; // Số bản ghi trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1

            // Lưu từ khóa tìm kiếm vào ViewBag để sử dụng lại trong View (giữ nguyên từ khóa trong ô input khi người dùng tìm kiếm)
            ViewBag.CurrentFilter = searchString;

            // Truy vấn danh sách Admin
            var admins = db.Admin.AsQueryable();

            // Nếu có từ khóa tìm kiếm thì lọc danh sách Admin
            if (!string.IsNullOrEmpty(searchString))
            {
                admins = admins.Where(a => a.SoDT.Contains(searchString) ||
                                           a.VaiTro.Contains(searchString));
            }

            // Sắp xếp danh sách Admin theo ID
            var adminList = admins.OrderBy(a => a.ID).ToPagedList(pageNumber, pageSize);

            return View(adminList);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admin.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Admins/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,HoTen,Email,DiaChi,SoDT,VaiTro,TKhoan,MKhau")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(admin);
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Admin admin = db.Admin.Find(id);
            if (admin == null) return HttpNotFound();

            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Admins");
            }
            return View(admin);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admin.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Admin admin = db.Admin.Find(id);
            db.Admin.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
