using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BanSach.Models;
using PagedList;
using Rotativa;

namespace BanSach.Controllers
{
    public class DonHangsController : Controller
    {
        private readonly DonHang _donHangService; // Dịch vụ đơn hàng
        private readonly db_Book db = new db_Book();
        public DonHangsController()
        {
            _donHangService = new DonHang(); // Hoặc khởi tạo một dịch vụ mặc định
        }
        public DonHangsController(DonHang donHangService)
        {
            _donHangService = donHangService;
        }
        public ActionResult Index(string searchString, DateTime? startDate, DateTime? endDate, int? page)
        {
            var donHangs = db.DonHang.Include(d => d.KhachHang).AsQueryable();

            // Tìm kiếm theo các tiêu chí
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

            // Phân trang
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var pagedDonHangs = donHangs.OrderBy(d => d.TrangThai).ToPagedList(pageNumber, pageSize);

            // Trả về View với kiểu IPagedList<DonHang>
            return View(pagedDonHangs);
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
                    TempData["ErrorMessage"] = "Đơn đặt hàng không được tìm thấy.\r\n";
                    return RedirectToAction("Index");
                }

                // Xoá các bản ghi liên quan trong DonHangCT
                var relatedOrderDetails = db.DonHangCT.Where(dhct => dhct.IDDonHang == id).ToList();
                db.DonHangCT.RemoveRange(relatedOrderDetails);

                // Xoá DonHang
                db.DonHang.Remove(donHang);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đơn đặt hàng và các chi tiết liên quan đã được xóa thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cố gắng xóa đơn hàng: " + ex.Message;
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

            // Gửi email thông báo cho admin về đơn hàng mới
            SendOrderNotificationEmail(id);

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
        public async Task<ActionResult> DaNhanHang(int id)
        {
            var donHang = await db.DonHang.FindAsync(id); // Sử dụng FindAsync cho bất đồng bộ
            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Cập nhật ngày nhận hàng và trạng thái khi người dùng đã nhận hàng
            donHang.NgayNhanHang = DateTime.Now;
            donHang.TrangThai = "Đã nhận hàng";
            await db.SaveChangesAsync(); // Sử dụng SaveChangesAsync để lưu bất đồng bộ

            // Chuyển hướng về trang lịch sử đơn hàng sau khi cập nhật
            return RedirectToAction("LichSuDonHang", "KhachHangs");
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult ThongKeDonHang()
        {
            // Lấy danh sách tất cả đơn hàng đã nhận
            var donHangsDaNhanHang = db.DonHang
                .Include(dh => dh.DonHangCT) // Bao gồm các chi tiết đơn hàng
                .Where(dh => dh.TrangThai == "Đã nhận hàng")
                .ToList(); // Đưa tất cả dữ liệu vào bộ nhớ

            // Tính tổng doanh thu từ các đơn hàng đã nhận
            decimal tongDoanhThu = donHangsDaNhanHang.Sum(dh => dh.totalamount ?? 0);

            // Khởi tạo ViewModel
            var thongKeViewModel = new ThongKeDonHangViewModel
            {
                TongSoDonHang = db.DonHang.Count(),
                DonHangChoXuLy = db.DonHang.Count(dh => dh.TrangThai == "Chờ xử lý"),
                DonHangDaXacNhan = db.DonHang.Count(dh => dh.TrangThai == "Đã xác nhận"),
                DonHangDaNhanHang = donHangsDaNhanHang.Count(),
                DonHangDaHuy = db.DonHang.Count(dh => dh.TrangThai == "Đã huỷ"),
                TongDoanhThu = tongDoanhThu
            };

            return View(thongKeViewModel);
        }

        // GET: DonHangs/RequestReturn/5
        public ActionResult RequestReturn(int id)
        {
            var donHang = db.DonHang.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Cập nhật trạng thái đơn hàng thành "Yêu cầu đổi trả"
            donHang.TrangThai = "Yêu cầu đổi trả";
            db.Entry(donHang).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult PrintInvoice(int id)
        {
            // Tìm đơn hàng và thông tin khách hàng liên quan
            var donHang = db.DonHang.Include(d => d.KhachHang).FirstOrDefault(d => d.IDdh == id);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Sử dụng Rotativa để tạo tệp PDF từ view "Invoice"
            return new Rotativa.ViewAsPdf("Inhoadon", donHang)
            {
                FileName = $"HoaDon_{donHang.IDdh}.pdf", // Tên tệp PDF
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }
        // Action xác nhận tất cả các đơn hàng có trạng thái "Chờ xử lý"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmAll()
        {
            try
            {
                // Lấy danh sách tất cả các đơn hàng có trạng thái "Chờ xử lý"
                var donHangsToConfirm = db.DonHang.Where(dh => dh.TrangThai == "Chờ xử lý").ToList();

                foreach (var donHang in donHangsToConfirm)
                {
                    // Cập nhật trạng thái đơn hàng
                    donHang.TrangThai = "Đã xác nhận";
                    db.Entry(donHang).State = EntityState.Modified;
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                TempData["SuccessMessage"] = "Tất cả các đơn hàng đã được xác nhận.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xác nhận tất cả đơn hàng: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
        public void SendOrderNotificationEmail(int orderId)
        {
            // Tìm đơn hàng từ cơ sở dữ liệu
            var donHang = db.DonHang.Include(d => d.KhachHang).FirstOrDefault(d => d.IDdh == orderId);

            if (donHang != null)
            {
                // Thông tin khách hàng và nội dung email
                string customerEmail = donHang.KhachHang.Email; // Địa chỉ email của khách hàng
                string subject = $"Đơn hàng {donHang.IDdh} của bạn đã được xác nhận";

                // Nội dung email với HTML
                string body = $@"
        <html>
        <body>
            <h2>Xin chào {donHang.KhachHang.TenKH},</h2>
            <p>Đơn hàng của bạn đã được xác nhận.</p>
            <p><strong>Thông tin đơn hàng:</strong></p>
            <table border='1' cellpadding='5' cellspacing='0'>
                <tr><td><strong>Mã đơn hàng:</strong></td><td>{donHang.IDdh}</td></tr>
                <tr><td><strong>Ngày đặt hàng:</strong></td><td>{donHang.NgayDatHang?.ToString("dd/MM/yyyy HH:mm:ss")}</td></tr>
                <tr><td><strong>Trạng thái:</strong></td><td>{donHang.TrangThai}</td></tr>
                <tr><td><strong>Tổng tiền:</strong></td><td>{donHang.totalamount?.ToString("C")}</td></tr>
                <tr><td><strong>Địa chỉ giao hàng:</strong></td><td>{donHang.DiaChi}</td></tr>
            </table>
            <p><strong>Chi tiết sản phẩm:</strong></p>
            <ul>";

                // Thêm thông tin chi tiết đơn hàng
                foreach (var item in donHang.DonHangCT)
                {
                    body += $@"
                <li>{item.SanPham.TenSP}, Số lượng: {item.SoLuong}, Đơn giá: {item.Gia?.ToString("C")}, Thành tiền: {(item.SoLuong * (decimal)(item.Gia ?? 0))}</li>";
                }

                body += $@"
            </ul>
            <p>Cảm ơn bạn đã mua hàng tại chúng tôi!</p>
        </body>
        </html>";

                // Cấu hình SmtpClient cho Gmail
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587) // Sử dụng SMTP của Gmail
                {
                    EnableSsl = true, // Bật SSL
                    Credentials = new System.Net.NetworkCredential("crandi21112004@gmail.com", "lnragtcdumizfwwq"), // Sử dụng mật khẩu ứng dụng
                    Timeout = 30000 // Thời gian chờ kết nối (30 giây)
                };

                // Tạo đối tượng MailMessage
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("crandi21112004@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Đặt là true để gửi email dưới dạng HTML
                };

                // Thêm người nhận (địa chỉ email của khách hàng)
                mailMessage.To.Add(customerEmail);

                try
                {
                    // Gửi email
                    smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu có
                    Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                }
            }
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
