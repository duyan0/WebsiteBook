using BanSach.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class KhuyenMaiController : Controller
    {

        private readonly db_Book db = new db_Book();

        [HttpGet]
        public PartialViewResult GetPromotion()
        {
            var GetPromotion = db.KhuyenMai.ToList(); // Sắp xếp theo thứ tự
            return PartialView(GetPromotion);
        }
        public ActionResult Index(string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;

            // Fetch all KhuyenMai records from the database
            var promotions = db.KhuyenMai
                               .Include(km => km.SanPham) // Include related products
                               .Select(km => new PromotionViewModel
                               {
                                   IDkm = km.IDkm,
                                   TenKhuyenMai = km.TenKhuyenMai,
                                   NgayBatDau = km.NgayBatDau,
                                   NgayKetThuc = km.NgayKetThuc,
                                   MucGiamGia = (int)km.MucGiamGia,
                                   MoTa = km.MoTa,
                                   SanPhamCount = km.SanPham.Count() // Calculate the number of products using the promotion
                               });

            // Apply search filter if searchString is provided
            if (!String.IsNullOrEmpty(searchString))
            {
                promotions = promotions.Where(k => k.TenKhuyenMai.Contains(searchString));
            }

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
            // Tìm đối tượng KhuyenMai theo id
            KhuyenMai km = db.KhuyenMai.Find(id);
            if (km != null)
            {
                // Kiểm tra nếu có sản phẩm liên kết với khuyến mãi này
                var sanPhams = db.SanPham.Where(s => s.IDkm == id).ToList();

                if (sanPhams.Any()) // Nếu có sản phẩm liên kết
                {
                    // Thay vì xóa, bạn có thể bỏ liên kết các sản phẩm với khuyến mãi
                    foreach (var sp in sanPhams)
                    {
                        sp.KhuyenMai.MucGiamGia = 0; // Hoặc bạn có thể gán giá trị khác tùy theo yêu cầu
                    }

                    db.SaveChanges(); // Lưu lại thay đổi

                    // Hiển thị cảnh báo và không xóa khuyến mãi
                    ViewBag.ErrorMessage = "Không thể xóa khuyến mãi vì có sản phẩm đang áp dụng khuyến mãi này.";
                    return View(km); // Trả về view chi tiết khuyến mãi, không xóa
                }
                else
                {
                    // Nếu không có sản phẩm liên kết, tiến hành xóa khuyến mãi
                    db.KhuyenMai.Remove(km);
                    db.SaveChanges();

                    ViewBag.SuccessMessage = "Xoá thành công!";
                    return RedirectToAction("Index"); // Redirect về danh sách khuyến mãi
                }
            }
            else
            {
                // Nếu không tìm thấy khuyến mãi
                ViewBag.ErrorMessage = "Không tìm thấy thông tin khuyến mãi.";
                return RedirectToAction("Index");
            }
        }
        public ActionResult XoaKhuyenMaiHetHan()
        {
            // Lọc tất cả các chương trình khuyến mãi đã hết hạn trước ngày hiện tại
            var khuyenMaiHetHan = db.KhuyenMai.Where(km => km.NgayKetThuc < DateTime.Now).ToList();

            if (khuyenMaiHetHan.Any())
            {
                foreach (var khuyenMai in khuyenMaiHetHan)
                {
                    // Kiểm tra xem có sản phẩm nào còn liên kết với khuyến mãi này không
                    var sanPhamsLienKet = db.SanPham.Any(sp => sp.IDkm == khuyenMai.IDkm);

                    if (sanPhamsLienKet)
                    {
                        TempData["ErrorMessage"] = $"Không thể xóa khuyến mãi {khuyenMai.TenKhuyenMai} vì có sản phẩm liên kết.";
                        continue;
                    }

                    // Nếu không có sản phẩm liên kết, tiếp tục xóa
                    db.KhuyenMai.Remove(khuyenMai);
                }

                db.SaveChanges();
                TempData["ThongBaoThanhCong"] = "Xóa thành công tất cả các chương trình khuyến mãi đã hết hạn không có liên kết với sản phẩm!";
            }
            else
            {
                TempData["ThongBaoLoi"] = "Không có chương trình khuyến mãi nào đã hết hạn.";
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