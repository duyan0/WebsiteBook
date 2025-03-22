using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class BannerController : Controller
    {
        private readonly db_Book db = new db_Book();

        // Giải phóng tài nguyên khi controller bị hủy
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Banners (Hiển thị danh sách banner)
        [HttpGet]
        public PartialViewResult Banners()
        {
            var banners = db.Banner.OrderBy(b => b.ThuTu).ToList(); // Sắp xếp theo thứ tự
            return PartialView(banners);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var banners = db.Banner.OrderBy(b => b.ThuTu).ToList();
            return View(banners);
        }

        // GET: Banner/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Banner/Create (Thêm mới banner với ràng buộc)
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật chống CSRF
        public ActionResult Create([Bind(Include = "HinhAnh,MoTa,Link,ThuTu")] Banner banner)
        {
            try
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(banner.HinhAnh))
                    ModelState.AddModelError("HinhAnh", "Hình ảnh là bắt buộc.");
                if (string.IsNullOrEmpty(banner.Link))
                    ModelState.AddModelError("Link", "Link là bắt buộc.");
                if (banner.ThuTu == null)
                    ModelState.AddModelError("ThuTu", "Thứ tự là bắt buộc.");

                // Ràng buộc độ dài và định dạng
                if (banner.Link != null && banner.Link.Length > 250)
                    ModelState.AddModelError("Link", "Link không được dài quá 250 ký tự.");
                if (banner.MoTa != null && banner.MoTa.Length > 500)
                    ModelState.AddModelError("MoTa", "Mô tả không được dài quá 500 ký tự.");
                if (banner.ThuTu < 0)
                    ModelState.AddModelError("ThuTu", "Thứ tự không được là số âm.");

                // Kiểm tra trùng thứ tự
                if (db.Banner.Any(b => b.ThuTu == banner.ThuTu))
                    ModelState.AddModelError("ThuTu", "Thứ tự này đã tồn tại.");

                if (ModelState.IsValid)
                {
                    db.Banner.Add(banner);
                    db.SaveChanges();
                    TempData["Success"] = "Thêm banner thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }
            return View(banner);
        }

        // GET: Banner/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var banner = db.Banner.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
        }

        // POST: Banner/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Banner_ID,HinhAnh,MoTa,Link,ThuTu")] Banner banner)
        {
            try
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(banner.HinhAnh))
                    ModelState.AddModelError("HinhAnh", "Hình ảnh là bắt buộc.");
                if (string.IsNullOrEmpty(banner.Link))
                    ModelState.AddModelError("Link", "Link là bắt buộc.");
                if (banner.ThuTu == null)
                    ModelState.AddModelError("ThuTu", "Thứ tự là bắt buộc.");

                if (banner.Link != null && banner.Link.Length > 250)
                    ModelState.AddModelError("Link", "Link không được dài quá 250 ký tự.");
                if (banner.MoTa != null && banner.MoTa.Length > 500)
                    ModelState.AddModelError("MoTa", "Mô tả không được dài quá 500 ký tự.");
                if (banner.ThuTu < 0)
                    ModelState.AddModelError("ThuTu", "Thứ tự không được là số âm.");

               

                if (ModelState.IsValid)
                {
                    var existingBanner = db.Banner.Find(banner.Banner_ID);
                    if (existingBanner == null)
                    {
                        return HttpNotFound(); // Bản ghi không tồn tại
                    }

                    // Cập nhật các giá trị
                    existingBanner.HinhAnh = banner.HinhAnh;
                    existingBanner.MoTa = banner.MoTa;
                    existingBanner.Link = banner.Link;
                    existingBanner.ThuTu = banner.ThuTu;

                    db.Entry(existingBanner).State = EntityState.Modified;
                    int rowsAffected = db.SaveChanges();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("Không thể cập nhật banner do dữ liệu đã bị thay đổi hoặc xóa.");
                    }

                    TempData["Success"] = "Cập nhật banner thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }
            return View(banner);
        }

        // POST: Banner/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var banner = db.Banner.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }

            db.Banner.Remove(banner);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
