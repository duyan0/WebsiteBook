using AngleSharp.Text;
using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class SlideController : Controller
    {
        private readonly db_Book db = new db_Book();

        // Giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            var slides = db.Slide.ToList();
            return View(slides);
        }
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult Silde()
        {
            var slide = db.Slide.ToList();
            return PartialView(slide);
        }
        // GET: Create
        [HttpGet]
        public ActionResult Create()
        {
            var usedThuTus = db.Slide.Select(sp => sp.ThuTu).ToList();
            ViewBag.UsedThuTus = usedThuTus ?? new List<int?>();
            return View(new Slide());
        }

        // POST: Create (Thêm mới Slide với upload hình ảnh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MoTa,Link,ThuTu,HinhAnh")] Slide slide, HttpPostedFileBase HinhAnh)
        {
            try
            {
                // Kiểm tra file upload
                if (HinhAnh != null && HinhAnh.ContentLength > 0)
                {
                    // Kiểm tra định dạng file (chỉ cho phép hình ảnh)
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(HinhAnh.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("HinhAnh", "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif).");
                    }
                    else
                    {
                        // Tạo tên file duy nhất để tránh trùng lặp
                        var fileName = Guid.NewGuid().ToString() + extension;
                        var path = Path.Combine(Server.MapPath("~/assets/images/slide/"), fileName);

                        // Lưu file vào thư mục
                        HinhAnh.SaveAs(path);

                        // Lưu đường dẫn vào model
                        slide.HinhAnh = "" + fileName;
                    }
                }
                else
                {
                    ModelState.AddModelError("HinhAnh", "Hình ảnh là bắt buộc.");
                }

                // Kiểm tra các trường bắt buộc khác
                if (string.IsNullOrEmpty(slide.Link))
                    ModelState.AddModelError("Link", "Link là bắt buộc.");
                if (slide.ThuTu == null)
                    ModelState.AddModelError("ThuTu", "Thứ tự là bắt buộc.");

                // Ràng buộc độ dài và định dạng
                if (slide.Link != null && slide.Link.Length > 250)
                    ModelState.AddModelError("Link", "Link không được dài quá 250 ký tự.");
                if (slide.MoTa != null && slide.MoTa.Length > 500)
                    ModelState.AddModelError("MoTa", "Mô tả không được dài quá 500 ký tự.");
                if (slide.ThuTu.HasValue && slide.ThuTu < 0)
                    ModelState.AddModelError("ThuTu", "Thứ tự không được là số âm.");

                // Kiểm tra trùng thứ tự
                if (slide.ThuTu.HasValue && db.Slide.Any(s => s.ThuTu == slide.ThuTu))
                    ModelState.AddModelError("ThuTu", "Thứ tự này đã tồn tại.");

                if (ModelState.IsValid)
                {
                    db.Slide.Add(slide);
                    db.SaveChanges();
                    TempData["Success"] = "Thêm Slide thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            // Trả lại danh sách thứ tự đã sử dụng nếu thất bại
            ViewBag.UsedThuTus = db.Slide.Select(s => s.ThuTu).ToList() ?? new List<int?>();
            return View(slide);
        }

        // GET: Edit/{id} (Hiển thị form sửa Slide)
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var slide = db.Slide.Find(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            ViewBag.UsedThuTus = db.Slide.Where(s => s.Slide_ID != id).Select(s => s.ThuTu).ToList() ?? new List<int?>();
            return View(slide);
        }

        // POST: Edit/{id} (Cập nhật Slide với ràng buộc)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Slide_ID,HinhAnh,MoTa,Link,ThuTu")] Slide slide)
        {
            try
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(slide.HinhAnh))
                    ModelState.AddModelError("HinhAnh", "Hình ảnh là bắt buộc.");
                if (string.IsNullOrEmpty(slide.Link))
                    ModelState.AddModelError("Link", "Link là bắt buộc.");
                if (slide.ThuTu == null)
                    ModelState.AddModelError("ThuTu", "Thứ tự là bắt buộc.");

                // Ràng buộc độ dài và định dạng
                if (slide.Link != null && slide.Link.Length > 250)
                    ModelState.AddModelError("Link", "Link không được dài quá 250 ký tự.");
                if (slide.MoTa != null && slide.MoTa.Length > 500)
                    ModelState.AddModelError("MoTa", "Mô tả không được dài quá 500 ký tự.");
                if (slide.ThuTu < 0)
                    ModelState.AddModelError("ThuTu", "Thứ tự không được là số âm.");

                if (ModelState.IsValid)
                {
                    var existingSlide = db.Slide.Find(slide.Slide_ID);
                    if (existingSlide == null)
                    {
                        return HttpNotFound();
                    }

                    // Cập nhật thủ công để tránh lỗi concurrency
                    existingSlide.HinhAnh = slide.HinhAnh;
                    existingSlide.MoTa = slide.MoTa;
                    existingSlide.Link = slide.Link;
                    existingSlide.ThuTu = slide.ThuTu;

                    db.Entry(existingSlide).State = EntityState.Modified;
                    int rowsAffected = db.SaveChanges();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("Không thể cập nhật Slide do dữ liệu đã bị thay đổi hoặc xóa.");
                    }

                    TempData["Success"] = "Cập nhật Slide thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            ViewBag.UsedThuTus = db.Slide.Where(s => s.Slide_ID != slide.Slide_ID).Select(s => s.ThuTu).ToList() ?? new List<int?>();
            return View(slide);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            var banner = db.Slide.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }

            db.Slide.Remove(banner);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}