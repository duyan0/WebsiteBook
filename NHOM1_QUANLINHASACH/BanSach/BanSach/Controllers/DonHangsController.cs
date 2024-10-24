using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BanSach.Models;

namespace BanSach.Controllers
{
    public class DonHangsController : Controller
    {
        private readonly DonHang _donHangService; // Dịch vụ đơn hàng
        private readonly SachEntities1 db = new SachEntities1(); // DbContext
        public DonHangsController()
        {
            _donHangService = new DonHang(); // Hoặc khởi tạo một dịch vụ mặc định
        }
        public DonHangsController(DonHang donHangService)
        {
            _donHangService = donHangService;
        }

        // GET: DonHangs
        public ActionResult Index()
        {
            var donHangs = db.DonHang.Include(d => d.KhachHang);
            return View(donHangs.ToList());
        }

        // GET: DonHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHang.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        // GET: DonHangs/Create
        public ActionResult Create()
        {
            ViewBag.IDkh = new SelectList(db.KhachHang, "IDkh", "TenKH");
            return View();
        }

        // POST: DonHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDdh,NgayDatHang,IDkh,DiaChi,NgayNhanHang")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.DonHang.Add(donHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDkh = new SelectList(db.KhachHang, "IDkh", "TenKH", donHang.IDkh);
            return View(donHang);
        }

        // GET: DonHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHang.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDkh = new SelectList(db.KhachHang, "IDkh", "TenKH", donHang.IDkh);
            return View(donHang);
        }

        // POST: DonHangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDdh,NgayDatHang,IDkh,DiaChi,NgayNhanHang")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDkh = new SelectList(db.KhachHang, "IDkh", "TenKH", donHang.IDkh);
            return View(donHang);
        }

        // GET: DonHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHang.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        // POST: DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DonHang donHang = db.DonHang.Find(id);
            db.DonHang.Remove(donHang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: DonHangs/Confirm/5
        public ActionResult Confirm(int id)
        {
            var donHang = db.DonHang.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            donHang.TrangThai = "Đã xác nhận"; // Cập nhật trạng thái đơn hàng
            db.Entry(donHang).State = EntityState.Modified; // Đánh dấu đơn hàng là đã sửa đổi
            db.SaveChanges(); // Lưu thay đổi

            return RedirectToAction("Index"); // Chuyển hướng về trang danh sách đơn hàng
        }

        // GET: DonHangs/Cancel/5
        public ActionResult Cancel(int id)
        {
            var donHang = db.DonHang.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            donHang.TrangThai = "Đã huỷ"; // Cập nhật trạng thái đơn hàng
            db.Entry(donHang).State = EntityState.Modified; // Đánh dấu đơn hàng là đã sửa đổi
            db.SaveChanges(); // Lưu thay đổi

            return RedirectToAction("Index"); // Chuyển hướng về trang danh sách đơn hàng
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
