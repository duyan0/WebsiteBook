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
    public class DanhMucsController : Controller
    {
        private readonly db_Book db = new db_Book();

        // Action PartialViewResult
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult PhanDanhMuc()
        {
            var cateList = db.DanhMuc_TheLoai.ToList();
            return PartialView(cateList);
        }
        public ActionResult Index()
        {
            var DanhMuc = db.DanhMuc.ToList();
            return View(DanhMuc);
        }
        public ActionResult Create()
        {
            var DM = db.DanhMuc.ToList();
            if (DM != null && DM.Count > 0)
            {
                ViewBag.DM = new SelectList(DM, "ID", "TenDanhMuc");
            }
            else
            {
                ViewBag.DM = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DanhMuc model)
        {
            // Kiểm tra TenDanhMuc không trùng (không phân biệt hoa/thường)
            if (db.DanhMuc.Any(d => d.TenDanhMuc.ToLower() == model.TenDanhMuc.ToLower()))
            {
                ModelState.AddModelError("TenDanhMuc", "Tên danh mục đã tồn tại. Vui lòng chọn tên khác.");
            }

            if (ModelState.IsValid)
            {
                // Lưu model vào cơ sở dữ liệu
                db.DanhMuc.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, tải lại danh sách danh mục
            var DM = db.DanhMuc.ToList();
            ViewBag.DM = new SelectList(DM, "ID", "TenDanhMuc", model.ID);
            return View(model);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DanhMuc DM = db.DanhMuc.Find(id);  // Tìm tác giả dựa trên ID
            if (DM == null)
            {
                return HttpNotFound();
            }

            return View(DM);  // Truyền model TacGia vào View để hiển thị
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DanhMuc DM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(DM).State = EntityState.Modified;  // Đánh dấu đối tượng là đã sửa đổi
                db.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction("Index");  // Chuyển hướng về trang danh sách sau khi cập nhật thành công
            }

            return View(DM);  // Nếu không hợp lệ, trả lại form chỉnh sửa với dữ liệu hiện có
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);  // Trả về lỗi nếu ID không hợp lệ
            }

            DanhMuc DM = db.DanhMuc.Find(id);  // Tìm đối tượng TacGia dựa vào ID
            if (DM == null)
            {
                return HttpNotFound();  // Nếu không tìm thấy, trả về lỗi 404
            }

            return View(DM);  // Trả về view và truyền model TacGia vào view để hiển thị chi tiết
        }
        // GET: TacGia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DanhMuc DM = db.DanhMuc.Find(id);
            if (DM == null)
            {
                return HttpNotFound();
            }

            return View(DM);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhMuc DM = db.DanhMuc.Find(id);
            db.DanhMuc.Remove(DM);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public PartialViewResult Banner()
        {
            var cateList = db.DanhMuc_TheLoai.Take(6).ToList(); // Lấy 6 mục đầu tiên
            return PartialView(cateList);
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


