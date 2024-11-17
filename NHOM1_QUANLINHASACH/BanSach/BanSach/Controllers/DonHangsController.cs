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
        private readonly dbSach db = new dbSach(); // DbContext
        public DonHangsController()
        {
            _donHangService = new DonHang(); // Hoặc khởi tạo một dịch vụ mặc định
        }
        public DonHangsController(DonHang donHangService)
        {
            _donHangService = donHangService;
        }
        public ActionResult Index()
        {
            var donHangs = db.DonHang.Include(d => d.KhachHang);
            return View(donHangs.ToList());
        }
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
        public ActionResult Create()
        {
            ViewBag.IDkh = new SelectList(db.KhachHang, "IDkh", "TenKH");
            return View();
        }
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                DonHang donHang = db.DonHang.Find(id);

                if (donHang == null)
                {
                    TempData["ErrorMessage"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                // Xoá các bản ghi liên quan trong DonHangCT
                var relatedOrderDetails = db.DonHangCT.Where(dhct => dhct.IDDonHang == id).ToList();
                db.DonHangCT.RemoveRange(relatedOrderDetails);

                // Xoá DonHang
                db.DonHang.Remove(donHang);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Order and its related details deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while trying to delete the order: " + ex.Message;
                return RedirectToAction("Index");
            }
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
        [HttpGet]
        [Route("DonHang/DaNhanHang/{id:int}")]
        public ActionResult DaNhanHang(int id)
        {
            var donHang = db.DonHang.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            donHang.NgayNhanHang = DateTime.Now;
            donHang.TrangThai = "Đã nhận hàng";
            db.SaveChanges();

            return RedirectToAction("LichSuDonHang", "KhachHangs");
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
