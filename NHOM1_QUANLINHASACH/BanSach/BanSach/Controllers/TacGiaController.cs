using BanSach.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class TacGiaController : Controller
    {
        // GET: TacGia
        private readonly db_book1 db = new db_book1();
        public ActionResult Index(string searchString, int? page)
        {
            // Query to get authors from the database
            var authors = db.TacGia.AsQueryable();

            // Filtering logic if search string is provided
            if (!String.IsNullOrEmpty(searchString))
            {
                authors = authors.Where(a => a.TenTG.Contains(searchString));
            }

            // Set page size and page number
            int pageSize = 10; // Number of authors per page
            int pageNumber = (page ?? 1); // Default to page 1 if page is null

            // Use ToPagedList to paginate the list of authors
            return View(authors.OrderBy(a => a.IDtg).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            // Tạo danh sách quốc gia
            ViewBag.QuocGia = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Value = "VN", Text = "Việt Nam" },
                        new SelectListItem { Value = "US", Text = "Mỹ" },
                        new SelectListItem { Value = "JP", Text = "Nhật Bản" },
                        new SelectListItem { Value = "FR", Text = "Pháp" },
                    }, "Value", "Text");

            // Lấy danh sách tác giả từ cơ sở dữ liệu
            var tacGias = db.TacGia.ToList();

            // Kiểm tra xem danh sách tác giả có dữ liệu không
            if (tacGias != null && tacGias.Count > 0)
            {
                ViewBag.TacGia = new SelectList(tacGias, "IDtg", "TenTG");
            }
            else
            {
                ViewBag.TacGia = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TacGia model) // Đổi lại đúng tên model của bảng sản phẩm (SanPham)
        {
            if (ModelState.IsValid)
            {
                // Lưu model vào cơ sở dữ liệu
                db.TacGia.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hãy gọi lại danh sách tác giả và quốc gia
            var tacGias = db.TacGia.ToList();
            ViewBag.TacGia = new SelectList(tacGias, "IDtg", "TenTG", model.IDtg); // Ghi lại chọn IDtg hiện tại

            ViewBag.QuocGia = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "VN", Text = "Việt Nam" },
                new SelectListItem { Value = "US", Text = "Mỹ" },
                new SelectListItem { Value = "JP", Text = "Nhật Bản" },
                new SelectListItem { Value = "FR", Text = "Pháp" },
            }, "Value", "Text", model.TenTG); // Giữ giá trị QuocGia hiện tại

            return View(model);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TacGia tg = db.TacGia.Find(id);  // Tìm tác giả dựa trên ID
            if (tg == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuocGia = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "VN", Text = "Việt Nam" },
                new SelectListItem { Value = "US", Text = "Mỹ" },
                new SelectListItem { Value = "JP", Text = "Nhật Bản" },
                new SelectListItem { Value = "FR", Text = "Pháp" },
                // Thêm các quốc gia khác ở đây
            }, "Value", "Text", tg.QuocGia); // Để chọn quốc gia hiện tại

            return View(tg); ;  // Truyền model TacGia vào View để hiển thị
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDtg,TenTacGia,NgaySinh,QuocGia,TieuSu")] TacGia tg)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tg).State = EntityState.Modified;  // Đánh dấu đối tượng là đã sửa đổi
                db.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction("Index");  // Chuyển hướng về trang danh sách sau khi cập nhật thành công
            }

            return View(tg);  // Nếu không hợp lệ, trả lại form chỉnh sửa với dữ liệu hiện có
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);  // Trả về lỗi nếu ID không hợp lệ
            }

            TacGia tg = db.TacGia.Find(id);  // Tìm đối tượng TacGia dựa vào ID
            if (tg == null)
            {
                return HttpNotFound();  // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(tg);  // Trả về view và truyền model TacGia vào view để hiển thị chi tiết
        }
        // GET: TacGia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TacGia tg = db.TacGia.Find(id);
            if (tg == null)
            {
                return HttpNotFound();
            }
            return View(tg);
        }
        // POST: TacGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var relatedProducts = db.SanPham.Where(sp => sp.IDtg == id).ToList();

            if (relatedProducts.Any()) // Nếu có sản phẩm liên quan
            {
                // Thêm thông báo lỗi vào ViewBag
                ViewBag.ErrorMessage = "Tác giả này đang được sử dụng và không thể xóa vì có sản phẩm liên quan.";
                return View("Delete", db.TacGia.Find(id)); // Trả lại view Delete với thông báo lỗi
            }
            TacGia tg = db.TacGia.Find(id);
            db.TacGia.Remove(tg);
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