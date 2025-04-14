using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using BanSach.DesignPatterns.StatePattern;
using BanSach.Models;
using PagedList;
using PayPal.Api;
using Rotativa;
using Stripe.Billing;

namespace BanSach.Controllers
{
    public class DonHangsController : Controller
    {
        private readonly db_Book _db;

        public DonHangsController()
        {
            _db = new db_Book();
        }

        public ActionResult Index(string searchString, DateTime? startDate, DateTime? endDate, int? page)
        {
            var donHangs = _db.DonHang.Include(d => d.KhachHang).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                donHangs = donHangs.Where(d => d.IDdh.ToString().Contains(searchString)
                                             || d.KhachHang.TenKH.Contains(searchString)
                                             || d.TrangThai.Contains(searchString));
            }

            if (startDate.HasValue)
            {
                donHangs = donHangs.Where(d => d.NgayDatHang >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                donHangs = donHangs.Where(d => d.NgayDatHang <= endDate.Value);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var pagedDonHangs = donHangs.OrderBy(d => d.TrangThai).ToPagedList(pageNumber, pageSize);

            return View(pagedDonHangs);
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            DonHang donHang = _db.DonHang.Find(id);
            if (donHang == null) return HttpNotFound();
            return View(donHang);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            DonHang donHang = _db.DonHang.Find(id);
            if (donHang == null) return HttpNotFound();
            return View(donHang);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                DonHang donHang = _db.DonHang.Find(id);
                if (donHang == null)
                {
                    TempData["ErrorMessage"] = "Đơn đặt hàng không được tìm thấy.";
                    return RedirectToAction("Index");
                }

                var relatedOrderDetails = _db.DonHangCT.Where(dhct => dhct.IDDonHang == id).ToList();
                _db.DonHangCT.RemoveRange(relatedOrderDetails);
                _db.DonHang.Remove(donHang);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Đơn đặt hàng và các chi tiết liên quan đã được xóa thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cố gắng xóa đơn hàng: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public ActionResult Confirm(int id)
        {
            var donHang = _db.DonHang.Find(id);
            if (donHang == null) return HttpNotFound();

            try
            {
                donHang.State.UpdateStatus(donHang, _db);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xác nhận đơn hàng: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Cancel(int id)
        {
            var donHang = _db.DonHang.Find(id);
            if (donHang == null) return HttpNotFound();

            try
            {
                donHang.TrangThai = "Đã huỷ";
                donHang.State = new CancelledState();
                _db.Entry(donHang).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi hủy đơn hàng: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Route("DonHang/DaNhanHang/{id:int}")]
        public ActionResult DaNhanHang(int id)
        {
            var donHang = _db.DonHang.Find(id);
            if (donHang == null) return HttpNotFound();

            try
            {
                // Khởi tạo State dựa trên TrangThai
                donHang.InitializeState();

                // Kiểm tra trạng thái hiện tại của đơn hàng
                if (donHang.TrangThai != "Đã xác nhận")
                {
                    TempData["ErrorMessage"] = "Đơn hàng không ở trạng thái 'Đã xác nhận', không thể xác nhận đã nhận hàng.";
                    return RedirectToAction("LichSuDonHang", "KhachHangs");
                }

                // Cập nhật trạng thái
                donHang.State.UpdateStatus(donHang, _db);

                // Thêm thông báo thành công
                TempData["SuccessMessage"] = "Xác nhận đã nhận hàng thành công!";
                return RedirectToAction("LichSuDonHang", "KhachHangs");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xác nhận đã nhận hàng: {ex.Message}";
                return RedirectToAction("LichSuDonHang", "KhachHangs");
            }
        }
        
        public ActionResult ThongKeDonHang()
        {
            var donHangsDaNhanHang = _db.DonHang
                .Include(dh => dh.DonHangCT)
                .Where(dh => dh.TrangThai == "Đã nhận hàng")
                .ToList();

            decimal tongDoanhThu = donHangsDaNhanHang.Sum(dh => dh.Total_DH);

            var thongKeViewModel = new ThongKeDonHangViewModel
            {
                TongSoDonHang = _db.DonHang.Count(),
                DonHangChoXuLy = _db.DonHang.Count(dh => dh.TrangThai == "Chờ xử lý"),
                DonHangDaXacNhan = _db.DonHang.Count(dh => dh.TrangThai == "Đã xác nhận"),
                DonHangDaNhanHang = donHangsDaNhanHang.Count(),
                DonHangDaHuy = _db.DonHang.Count(dh => dh.TrangThai == "Đã huỷ"),
                TongDoanhThu = tongDoanhThu
            };

            return View(thongKeViewModel);
        }

        public ActionResult RequestReturn(int id)
        {
            var donHang = _db.DonHang.Find(id);
            if (donHang == null) return HttpNotFound();

            try
            {
                donHang.TrangThai = "Yêu cầu trả hàng";
                donHang.State = new ReturnRequestedState();
                _db.Entry(donHang).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi yêu cầu trả hàng: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public ActionResult PrintInvoice(int id)
        {
            var donHang = _db.DonHang.Include(d => d.KhachHang).FirstOrDefault(d => d.IDdh == id);
            if (donHang == null) return HttpNotFound();

            return new Rotativa.ViewAsPdf("Inhoadon", donHang)
            {
                FileName = $"HoaDon_{donHang.IDdh}.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmAll()
        {
            try
            {
                var donHangsToConfirm = _db.DonHang.Where(dh => dh.TrangThai == "Chờ xử lý").ToList();
                foreach (var donHang in donHangsToConfirm)
                {
                    donHang.State.UpdateStatus(donHang, _db);
                }
                TempData["SuccessMessage"] = "Tất cả các đơn hàng đã được xác nhận.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xác nhận tất cả đơn hàng: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}



