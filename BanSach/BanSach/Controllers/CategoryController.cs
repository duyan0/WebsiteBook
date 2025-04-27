using BanSach.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class CategoryController : Controller
    {
        // GET: TheLoai
        private readonly db_Book db = new db_Book();
        [Route("category")]
        public ActionResult Index()
        {
            var theloai = db.TheLoai.ToList();
            return View(theloai);
        }
        public ActionResult GetCategory()
        {
            var theloai = db.TheLoai.ToList();
            return View(theloai);
        }
        public ActionResult Create()
        {
            var tl = db.TheLoai.ToList();
            if (tl != null && tl.Count > 0)
            {
                ViewBag.tl = new SelectList(tl, "IDtl", "TenTheLoai");
            }
            else
            {
                ViewBag.tl = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TheLoai model) // Đổi ModelType với tên model bạn đang sử dụng
        {
            if (ModelState.IsValid)
            {
                // Lưu model vào cơ sở dữ liệu
                db.TheLoai.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hãy gọi lại danh sách tác giả
            var tl = db.TheLoai.ToList();
            ViewBag.TL = new SelectList(tl, "IDtl", "TenTheLoai", model.ID); // Ghi lại chọn IDtg hiện tại
            return View(model);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TheLoai tl = db.TheLoai.Find(id);  // Tìm tác giả dựa trên ID
            if (tl == null)
            {
                return HttpNotFound();
            }

            return View(tl);  // Truyền model TacGia vào View để hiển thị
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TheLoai tl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tl).State = EntityState.Modified;  // Đánh dấu đối tượng là đã sửa đổi
                db.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction("Index");  // Chuyển hướng về trang danh sách sau khi cập nhật thành công
            }

            return View(tl);  // Nếu không hợp lệ, trả lại form chỉnh sửa với dữ liệu hiện có
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);  // Trả về lỗi nếu ID không hợp lệ
            }

            TheLoai tl = db.TheLoai.Find(id);  // Tìm đối tượng TacGia dựa vào ID
            if (tl == null)
            {
                return HttpNotFound();  // Nếu không tìm thấy, trả về lỗi 404
            }

            return View(tl);  // Trả về view và truyền model TacGia vào view để hiển thị chi tiết
        }
        // GET: TacGia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TheLoai tl = db.TheLoai.Find(id);
            if (tl == null)
            {
                return HttpNotFound();
            }

            return View(tl);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TheLoai tl = db.TheLoai.Find(id);

            // Kiểm tra xem có sản phẩm nào liên kết với nhà xuất bản không
            if (db.SanPham.Any(s => s.IDtl == id))
            {
                ModelState.AddModelError("", "Không thể xóa thể loại vì còn sản phẩm liên quan.");
                return View(tl); // Trả về view xóa với thông báo lỗi
            }

            db.TheLoai.Remove(tl);
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