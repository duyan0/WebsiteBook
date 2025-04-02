using BanSach.Models;

using ClosedXML.Excel;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace BanSach.Controllers
{

    public class KhachHangsController : Controller
    {
        private readonly db_Book db = new db_Book();
        public ActionResult LoginCus()
        {
            return View();
        }

        // GET: KhachHangs
        [HttpGet]
        public ActionResult Index()
        {
            var listUser = db.KhachHang.ToList();
            return View(listUser);
        }

        // GET: KhachHangs/Details/5
        // GET: KhachHangs/Details/5
        // GET: KhachHangs/Details/5
        public ActionResult Details(int? id)
        {
            // Kiểm tra nếu id không hợp lệ
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Kiểm tra xem session của khách hàng đã đăng nhập có tồn tại không
            if (Session["IDkh"] == null)
            {
                // Nếu không có session IDkh, người dùng chưa đăng nhập
                // Chuyển hướng đến controller "Account" và action "Login"
                return RedirectToAction("LoginAccountCus", "LoginUser");
                // Hoặc chuyển hướng đến trang đăng nhập
            }

            // Kiểm tra nếu IDkh trong session và ID của khách hàng là khớp
            int sessionId = Convert.ToInt32(Session["IDkh"]);
            if (sessionId != id)
            {
                // Nếu ID khách hàng trong session không trùng với id được yêu cầu
               return RedirectToAction("Forbidden", "Home");
            }

            // Tìm kiếm thông tin khách hàng theo id
            KhachHang khachHang = db.KhachHang.Find(id);

            // Kiểm tra nếu không tìm thấy khách hàng
            if (khachHang == null)
            {
                return HttpNotFound();
            }

            // Trả về view với đối tượng khách hàng
            return View(khachHang);
        }


        public ActionResult DetailsAD(int? id)
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
            var khachHang = new KhachHang
            {
                TrangThaiTaiKhoan = "Hoạt động" 
            };

            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDkh,TenKH,SoDT,Email,TKhoan,MKhau,TrangThaiTaiKhoan")] KhachHang khachHang)
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
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang kh = db.KhachHang.Find(id);
            if (kh == null)
            {
                return HttpNotFound();
            }
            return View(kh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDkh,TenKH,SoDT,Email,TKhoan,MKhau")] KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kh).State = EntityState.Modified;

                // Nếu bạn cần ValidateOnSaveEnabled = false
                db.Configuration.ValidateOnSaveEnabled = false;

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("UpdateSuccesss", "KhachHangs");
                }
                catch (Exception ex)
                {
                    // Log lỗi để kiểm tra thêm chi tiết nếu cần
                    Console.WriteLine(ex.Message);
                }
            }

            return View(kh);
        }
        public ActionResult EditAD(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang kh = db.KhachHang.Find(id);
            if (kh == null)
            {
                return HttpNotFound();
            }
            return View(kh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAD([Bind(Include = "IDkh,TenKH,SoDT,Email,TKhoan,MKhau")] KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kh).State = EntityState.Modified;

                // Nếu bạn cần ValidateOnSaveEnabled = false
                db.Configuration.ValidateOnSaveEnabled = false;

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index", "KhachHangs");
                }
                catch (Exception ex)
                {
                    // Log lỗi để kiểm tra thêm chi tiết nếu cần
                    Console.WriteLine(ex.Message);
                }
            }

            return View(kh);
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
        [HttpGet]
        [OutputCache(Duration = 0)]
        public ActionResult LichSuDonHang(int? page)
        {
            // Lấy ID khách hàng từ session
            int? currentCustomerId = Session["IDkh"] as int?;

            if (!currentCustomerId.HasValue)
            {
                return RedirectToAction("LoginAccountCus", "LoginUser");
            }

            // Truy vấn các đơn hàng của khách hàng
            var orders = db.DonHang
                .Where(dh => dh.IDkh == currentCustomerId.Value)
                .OrderByDescending(dh => dh.NgayDatHang) // Sắp xếp theo ngày đặt hàng giảm dần
                .Include(dh => dh.DonHangCT) // Bao gồm các chi tiết đơn hàng
                .ToList();

            // Khởi tạo State cho từng đơn hàng
            foreach (var order in orders)
            {
                order.InitializeState();
            }

            // Phân loại đơn hàng theo trạng thái
            var confirmedOrders = orders.Where(dh => dh.TrangThai == "Đã xác nhận").ToList();
            var pendingOrders = orders.Where(dh => dh.TrangThai == "Chờ xử lý").ToList();
            var canceledOrders = orders.Where(dh => dh.TrangThai == "Đã huỷ").ToList();
            var receivedOrders = orders.Where(dh => dh.TrangThai == "Đã nhận hàng").ToList();

            // Gộp lại theo trạng thái đã phân loại
            var allOrders = confirmedOrders
                .Concat(pendingOrders)
                .Concat(canceledOrders)
                .Concat(receivedOrders)
                .ToList();

            // Sắp xếp theo ngày đặt hàng giảm dần
            allOrders = allOrders.OrderByDescending(dh => dh.NgayDatHang).ToList();

            // Phân trang
            int pageSize = 10; // Số đơn hàng mỗi trang
            int pageNumber = (page ?? 1);

            // Chuyển đổi từ List sang IPagedList
            var pagedOrders = allOrders.ToPagedList(pageNumber, pageSize);

            return View(pagedOrders);
        }

        [HttpPost]
        public ActionResult Cancel(int? id)  // Thay thế kiểu Int32 bằng kiểu nullable Int32
        {
            if (!id.HasValue)  // Kiểm tra nếu id không được truyền
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid order ID.");
            }

            var donHang = db.DonHang.Find(id.Value);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            donHang.TrangThai = "Đã huỷ"; // Cập nhật trạng thái đơn hàng
            db.Entry(donHang).State = EntityState.Modified; // Đánh dấu đơn hàng là đã sửa đổi
            db.SaveChanges(); // Lưu thay đổi

            return RedirectToAction("LichSuDonHang"); // Chuyển hướng về trang danh sách đơn hàng
        }


        public async Task<ActionResult> ExportToExcel()
        {
            int? currentCustomerId = Session["IDkh"] as int?;

            if (!currentCustomerId.HasValue)
            {
                return RedirectToAction("LoginAccountCus", "LoginUser");
            }

            var orders = await db.DonHang
                .Where(dh => dh.IDkh == currentCustomerId.Value)
                .Include(dh => dh.DonHangCT)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("LichSuDonHang");
                var currentRow = 1;

                // Tiêu đề cột
                worksheet.Cell(currentRow, 1).Value = "Mã đơn hàng";
                worksheet.Cell(currentRow, 2).Value = "Ngày đặt hàng";
                worksheet.Cell(currentRow, 3).Value = "Ngày nhận hàng";
                worksheet.Cell(currentRow, 4).Value = "Tổng số tiền";
                worksheet.Cell(currentRow, 5).Value = "Trạng thái";

                // Nội dung đơn hàng
                foreach (var order in orders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = order.IDdh;
                    worksheet.Cell(currentRow, 2).Value = order.NgayDatHang?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa đặt hàng";
                    worksheet.Cell(currentRow, 3).Value = order.TrangThai == "Đã nhận hàng" && order.NgayNhanHang.HasValue
                        ? order.NgayNhanHang.Value.ToString("dd/MM/yyyy HH:mm:ss")
                        : (order.TrangThai == "Đã xác nhận" && order.NgayDatHang.HasValue
                            ? "Dự kiến: " + order.NgayDatHang.Value.AddDays(2).ToString("dd/MM/yyyy")
                            : "Chưa có thông tin");
                    worksheet.Cell(currentRow, 4).Value = order.totalamount;
                    worksheet.Cell(currentRow, 5).Value = order.TrangThai;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LichSuDonHang.xlsx");
                }
            }
        }
        [HttpPost]
        public JsonResult LockAccount(int id)
        {
            var khachHang = db.KhachHang.Find(id);
            if (khachHang != null)
            {
                try
                {
                    khachHang.TrangThaiTaiKhoan = "Bị khoá"; // Set status to "Bị khoá" to lock the account
                    db.Entry(khachHang).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Tài khoản đã bị khóa." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi khi khóa tài khoản: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Không tìm thấy tài khoản." });
        }

        [HttpPost]
        public JsonResult UnlockAccount(int id)
        {
            var khachHang = db.KhachHang.Find(id);
            if (khachHang != null)
            {
                try
                {
                    khachHang.TrangThaiTaiKhoan = "Hoạt động"; // Set status to "Hoạt động" to unlock the account
                    db.Entry(khachHang).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Tài khoản đã được mở khóa." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi khi mở khóa tài khoản: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Không tìm thấy tài khoản." });
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // Action để xử lý việc đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem UserId có tồn tại trong Session không
                if (Session["IDkh"] == null)
                {
                    // Nếu không có UserId trong session, chuyển hướng đến trang đăng nhập
                    return RedirectToAction("LoginAccountCus", "LoginUser");
                }

                int userId = (int)Session["IDkh"];
                var user = db.KhachHang.SingleOrDefault(kh => kh.IDkh == userId);


                if (user != null)
                {
                    // Kiểm tra mật khẩu hiện tại
                    if (user.MKhau != model.CurrentPassword)
                    {
                        ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng.");
                        return View(model);
                    }

                    // Kiểm tra mật khẩu mới và xác nhận mật khẩu mới
                    if (model.NewPassword != model.ConfirmNewPassword)
                    {
                        ModelState.AddModelError("ConfirmNewPassword", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                        return View(model);
                    }

                    // Cập nhật mật khẩu mới
                    // Cập nhật mật khẩu mới
                    user.MKhau = model.NewPassword;
                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
                    return RedirectToAction("ChangePassword"); // Chuyển hướng đến trang thông tin cá nhân của người dùng
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy người dùng.");
                }
            }

            return View(model);
        }
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
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