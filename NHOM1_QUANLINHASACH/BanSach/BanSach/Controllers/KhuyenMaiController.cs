using BanSach.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class KhuyenMaiController : Controller
    {
        // GET: KhuyenMai
        // GET: NhaXuatBan
        dbSach db = new dbSach();
        public ActionResult Index(string searchString, int? page)
        {
            // Fetch all KhuyenMai records from the database
            var promotions = db.KhuyenMai.AsQueryable();

            // Apply search filter if searchString is provided
            if (!String.IsNullOrEmpty(searchString))
            {
                promotions = promotions.Where(k => k.TenKhuyenMai.Contains(searchString));
            }

            // Define the page size and number
            int pageSize = 10; // Number of items per page
            int pageNumber = (page ?? 1); // Default to page 1 if no page is specified

            // Return the paginated list to the view
            return View(promotions.OrderBy(k => k.IDkm).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            var km = db.KhuyenMai.Select(k => new { k.IDkm, k.MucGiamGia }).ToList();
            ViewBag.KM = km.Any()
                ? new SelectList(km, "IDkm", "MucGiamGia")
                : new SelectList(Enumerable.Empty<SelectListItem>());

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhuyenMai model)
        {
            if (ModelState.IsValid)
            {
                if (model.NgayBatDau.HasValue)
                {
                    // Đặt ngày kết thúc là 7 ngày sau ngày bắt đầu nếu ngày bắt đầu không phải là null
                    model.NgayKetThuc = model.NgayBatDau.Value.AddDays(7);
                }

                // Lưu model vào cơ sở dữ liệu
                db.KhuyenMai.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hãy gọi lại danh sách khuyến mãi
            var km = db.KhuyenMai.Select(k => new { k.IDkm, k.MucGiamGia }).ToList();
            ViewBag.KM = km.Any()
                ? new SelectList(km, "IDkm", "MucGiamGia", model.IDkm) // Ghi lại chọn IDkm hiện tại
                : new SelectList(Enumerable.Empty<SelectListItem>());

            return View(model);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            KhuyenMai km = db.KhuyenMai.Find(id);  // Tìm tác giả dựa trên ID
            if (km == null)
            {
                return HttpNotFound();
            }

            return View(km);  // Truyền model TacGia vào View để hiển thị
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDkm,TenKhuyenMai,NgayBatDau,NgayKetThuc,MucGiamGia,MoTa")] KhuyenMai km)
        {
            if (ModelState.IsValid)
            {
                db.Entry(km).State = EntityState.Modified;  // Đánh dấu đối tượng là đã sửa đổi
                db.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction("Index");  // Chuyển hướng về trang danh sách sau khi cập nhật thành công
            }

            return View(km);  // Nếu không hợp lệ, trả lại form chỉnh sửa với dữ liệu hiện có
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);  // Trả về lỗi nếu ID không hợp lệ
            }

            KhuyenMai km = db.KhuyenMai.Find(id);  // Tìm đối tượng TacGia dựa vào ID
            if (km == null)
            {
                return HttpNotFound();  // Nếu không tìm thấy, trả về lỗi 404
            }

            return View(km);
        }
        // GET: TacGia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            KhuyenMai km = db.KhuyenMai.Find(id);
            if (km == null)
            {
                return HttpNotFound();
            }

            return View(km);
        }
        // POST: TacGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KhuyenMai km = db.KhuyenMai.Find(id);
            if (km != null)
            {
                db.KhuyenMai.Remove(km);
                db.SaveChanges();
                ViewBag.SuccessMessage = "Xoá thành công!";
            }
            else
            {
                ViewBag.ErrorMessage = "Không tìm thấy thông tin khuyến mãi.";
            }

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