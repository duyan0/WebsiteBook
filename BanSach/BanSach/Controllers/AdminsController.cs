using BanSach.DesignPatterns.CommandPattern;
using BanSach.DesignPatterns.FactoryPattern;
using BanSach.DesignPatterns.RepositoryPattern;
using BanSach.DesignPatterns.ServicePattern;
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
        private readonly IAdminFactory _adminFactory;
        private readonly IAdminRepository _adminRepository;
        private readonly IAdminService _adminService;
        private readonly db_Book _db;

        public AdminsController()
        {
            _adminFactory = new AdminFactory(); // Khởi tạo Factory trực tiếp
            _db = _adminFactory.CreateDbContext(); // Khởi tạo DbContext
            _adminRepository = new AdminRepository(_db); // Truyền DbContext vào Repository
            _adminService = new AdminService(_adminRepository); // Truyền Repository vào Service
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            using (var db = _adminFactory.CreateDbContext())
            {
                Admin admin = db.Admin.Find(id);
                if (admin == null) return HttpNotFound();
                return View(admin);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Admin admin)
        {
            if (ModelState.IsValid)
            {
                using (var db = _adminFactory.CreateDbContext())
                {
                    db.Entry(admin).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Admins");
                }
            }
            return View(admin);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Admin admin = _adminRepository.GetAdminById(id.Value);
            if (admin == null)
            {
                return HttpNotFound();
            }

            return View(admin);
        }
        public ActionResult Index()
        {
            var _listAdmin =_db.Admin.ToList();
            return View(_listAdmin);
        }
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
                _adminService.CreateAdmin(admin);
                return RedirectToAction("Index");
            }
            return View(admin);
        }

        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new db_Book()) // Khởi tạo DbContext trực tiếp
            {
                var command = new DeleteAdminCommand(id.Value, db);
                Admin admin = command.GetAdmin();
                if (admin == null)
                {
                    return HttpNotFound();
                }
                return View(admin);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new db_Book()) // Khởi tạo DbContext trực tiếp
            {
                var command = new DeleteAdminCommand(id, db);
                command.Execute();
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose(); // Dispose DbContext được khởi tạo trong constructor
            }
            base.Dispose(disposing);
        }
    }
}