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
    public class KhachHangsController : Controller
    {
        private SachEntities1 db = new SachEntities1();
        public ActionResult LoginCus()
        {
            return View();
        }

        // GET: KhachHangs
        public ActionResult Index()
        {
            return View(db.KhachHang.ToList());
        }

        // GET: KhachHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // GET: KhachHangs/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDkh,TenKH,SoDT,Email,TKhoan,MKhau")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại chưa
                var existingEmail = db.KhachHang.FirstOrDefault(kh => kh.Email == khachHang.Email);
                if (existingEmail != null)
                {
                    // Nếu email đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("Email", "Email này đã tồn tại. Vui lòng sử dụng email khác.");
                    return View(khachHang);
                }

                // Kiểm tra xem số điện thoại đã tồn tại chưa
                var existingPhoneNumber = db.KhachHang.FirstOrDefault(kh => kh.SoDT == khachHang.SoDT);
                if (existingPhoneNumber != null)
                {
                    // Nếu số điện thoại đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("SoDT", "Số điện thoại này đã tồn tại. Vui lòng sử dụng số điện thoại khác.");
                    return View(khachHang);
                }

                // Nếu cả email và số điện thoại không tồn tại, thêm đối tượng mới vào cơ sở dữ liệu
                db.KhachHang.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(khachHang);
        }




        // GET: KhachHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: KhachHangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDkh,TenKH,SoDT,Email,TKhoan,MKhau,ConfirmPass")] KhachHang khachHang)
        {
            // Kiểm tra mật khẩu nhập lại
            if (khachHang.MKhau != khachHang.ConfirmPass)
            {
                ViewBag.Error = "Mật khẩu nhập lại không đúng";
                return View(khachHang); // Trả về view cùng với mô hình
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại chưa (trừ khách hàng hiện tại)
                var existingEmail = db.KhachHang.FirstOrDefault(kh => kh.Email == khachHang.Email && kh.IDkh != khachHang.IDkh);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email này đã tồn tại. Vui lòng sử dụng email khác.");
                    return View(khachHang); // Trả về view cùng với mô hình
                }

                // Kiểm tra xem số điện thoại đã tồn tại chưa (trừ khách hàng hiện tại)
                var existingPhoneNumber = db.KhachHang.FirstOrDefault(kh => kh.SoDT == khachHang.SoDT && kh.IDkh != khachHang.IDkh);
                if (existingPhoneNumber != null)
                {
                    ModelState.AddModelError("SoDT", "Số điện thoại này đã tồn tại. Vui lòng sử dụng số điện thoại khác.");
                    return View(khachHang); // Trả về view cùng với mô hình
                }

                // Nếu mọi điều kiện đều hợp lệ, cập nhật thông tin khách hàng
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UpdateSuccesss");
            }

            return View(khachHang); // Trả về view cùng với mô hình nếu không hợp lệ
        }


        [HttpGet]
        public ActionResult UpdateSuccesss()
        {
            return View();
        }
        // GET: KhachHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KhachHang khachHang = db.KhachHang.Find(id);
            db.KhachHang.Remove(khachHang);
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
