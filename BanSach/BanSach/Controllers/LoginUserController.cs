using BanSach.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Data.Entity.Validation;
namespace BanSach.Controllers
{
    public class LoginUserController : Controller
    {
        private readonly db_Book db =  new db_Book();
        // GET: LoginUser
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (Session["Idkh"] != null)
            {
                Session["Idkh"] = null;
                Session["IDkh"] = null;
                Session["TenKH"] = null;
                Session["SoDT"] = null;
                return RedirectToAction("LoginAccountCus", "LoginUser");
            }
            if (Session["IdQly"] != null)
            {
                Session.Clear();
            }
            return RedirectToAction("ProductList", "SanPhams");
        }
        [HttpGet]
        public ActionResult RegisterCus()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterCus(KhachHang _user, string ConfirmPass)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorRegister = "Vui lòng điền đầy đủ các thông tin.";
                return View();
            }

            // Kiểm tra số điện thoại phải là 10 số (chỉ chứa số)
            if (string.IsNullOrEmpty(_user.SoDT) || !_user.SoDT.All(char.IsDigit) || _user.SoDT.Length != 10)
            {
                ViewBag.ErrorRegister = "Số điện thoại phải là 10 số (chỉ chứa số, ví dụ: 0123456789).";
                return View();
            }

            // Kiểm tra trùng lặp tài khoản, số điện thoại, và email
            bool isAccountExist = db.KhachHang.Any(s => s.TKhoan == _user.TKhoan);
            bool isPhoneExist = db.KhachHang.Any(s => s.SoDT == _user.SoDT);
            bool isEmailExist = db.KhachHang.Any(s => s.Email == _user.Email);

            if (isAccountExist || isPhoneExist || isEmailExist)
            {
                ViewBag.ErrorRegister = isAccountExist ? "Tài khoản đã tồn tại!" :
                                           isPhoneExist ? "Số điện thoại đã tồn tại!" :
                                           "Email đã tồn tại!";
                return View();
            }

            // Kiểm tra mật khẩu khớp nhau
            if (_user.MKhau != ConfirmPass)
            {
                ViewBag.ErrorRegister = "Mật khẩu nhập lại không đúng!";
                return View();
            }

            // Lưu người dùng với trạng thái chưa xác nhận
            _user.TrangThaiTaiKhoan = "Chưa xác nhận";
            _user.create_date= DateTime.Now;
            db.KhachHang.Add(_user);
            db.SaveChanges();

            // Tạo và lưu mã OTP
            string otp = GenerateOtp();
            _user.OTP = otp;
            _user.OTPExpiry = DateTime.Now.AddMinutes(5);
            db.SaveChanges();

            // Gửi email chứa mã OTP
            SendEmail(_user.Email, "Xác nhận tài khoản", $"Mã OTP của bạn: {otp}");

            // Chuyển hướng đến trang xác nhận
            return RedirectToAction("VerifyAccount", new { userId = _user.IDkh, method = "otp" });
        }

        private string GenerateOtp()
        {
            return "2111";
        }

        [HttpGet]
        public ActionResult VerifyAccount(int userId, string method = "otp")
        {
            var user = db.KhachHang.FirstOrDefault(u => u.IDkh == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return View("VerifyAccount");
            }

            if (method == "email" && user.TrangThaiTaiKhoan == "Chưa xác nhận")
            {
                user.TrangThaiTaiKhoan = "Hoạt động";
                db.SaveChanges();
                TempData["SuccessMessage"] = "Email đã được xác nhận thành công!";
                return View("VerifyAccount");
            }

            return View("VerifyAccount", new VerifyOtpViewModel { UserId = userId });
        }

        [HttpPost]
        public ActionResult VerifyAccount(VerifyOtpViewModel model)
        {
            var user = db.KhachHang.FirstOrDefault(u => u.IDkh == model.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return View("VerifyAccount");
            }

            // Kiểm tra mã OTP
            if (user.OTP != model.EnteredOtp)
            {
                TempData["ErrorMessage"] = "Mã OTP không đúng.";
                return View("VerifyAccount", model);
            }

            if (user.OTPExpiry?.AddMinutes(5) < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Mã OTP đã hết hạn.";
                return View("VerifyAccount", model);
            }

            // Xác nhận tài khoản
            user.TrangThaiTaiKhoan = "Hoạt động";
            user.OTP = null;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Tài khoản đã được xác nhận thành công!";
            return View("VerifyAccount"); // Trả về view thay vì redirect ngay
        }

        [HttpGet]
        public ActionResult SignUpSuccess()
        {
            return View();
        }
        [HttpGet]
        public ActionResult QuanTri()
        {
            return View();
        }

        //.../auth/userinfo.email,.../auth/userinfo.profile	
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult LoginAccountCus(KhachHang _cus)
        {

            if (_cus.TKhoan == null)
            {

                return View();
            }

            // Tìm kiếm trong bảng Admin
            var checkAdmin = db.Admin.FirstOrDefault(x => x.TKhoan == _cus.TKhoan && x.MKhau == _cus.MKhau);
            if (checkAdmin != null)
            {
                // Nếu là quản trị viên, thiết lập session quản lý và chuyển đến trang Admin
                Session["IDQly"] = checkAdmin.ID;
                Session["TenQly"] = checkAdmin.HoTen;
                Session["TKQly"] = checkAdmin.TKhoan;
                Session["Vaitro"] = checkAdmin.VaiTro;

                return RedirectToAction("Index", "Admins");
            }

            // Tìm kiếm trong bảng KhachHang
            // Kiểm tra tài khoản khách hàng
            var checkAccount = db.KhachHang.FirstOrDefault(s => s.TKhoan == _cus.TKhoan);
            if (checkAccount == null)
            {
                // Nếu không tìm thấy tài khoản
                ViewBag.ErrorInfo = "Tài khoản không tồn tại";
                return View();
            }

            // Kiểm tra mật khẩu khách hàng
            var checkPassword = db.KhachHang.FirstOrDefault(s => s.TKhoan == _cus.TKhoan && s.MKhau == _cus.MKhau);
            if (checkPassword == null)
            {
                // Nếu mật khẩu sai
                ViewBag.ErrorInfo = "Sai mật khẩu";
                return View();
            }

            // Nếu tài khoản và mật khẩu đều hợp lệ, tiếp tục xử lý


            // Kiểm tra nếu tài khoản khách hàng bị khóa
            if (checkAccount.TrangThaiTaiKhoan != null)
            {
                if (checkAccount.TrangThaiTaiKhoan.Equals("Bị khoá", StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.ErrorInfo = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với quản trị viên.";
                    return View();
                }
                else if (checkAccount.TrangThaiTaiKhoan.Equals("Chưa xác nhận", StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.ErrorInfo = "Tài khoản của bạn chưa được xác nhận. Vui lòng kiểm tra email để xác nhận tài khoản.";
                    return View();
                }
            }


            // Nếu tài khoản khách hàng hợp lệ và không bị khóa, thiết lập session
            db.Configuration.ValidateOnSaveEnabled = false;
            Session["IDkh"] = checkAccount.IDkh;
            Session["MKhau"] = checkAccount.MKhau;
            Session["TenKH"] = checkAccount.TenKH;
            Session["SoDT"] = checkAccount.SoDT;

            return RedirectToAction("TrangChu", "SanPhams");

        }



        [HttpGet]
        public ActionResult SignInSuccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult InfoCustomer()
        {
            return RedirectToAction("Details", "KhachHangs", new { id = Session["IDkh"] });
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập tài khoản hoặc email!";
                return View();
            }

            // Tìm kiếm khách hàng theo tài khoản hoặc email
            var khachHang = db.KhachHang.FirstOrDefault(kh => kh.TKhoan == email || kh.Email == email);

            if (khachHang == null)
            {
                ViewBag.ErrorMessage = "Tài khoản hoặc email không tồn tại!";
                return View();
            }
            string newPassword = GenerateRandomPassword(8);
            try
            {
                // Cập nhật mật khẩu mới cho khách hàng
                khachHang.MKhau = newPassword;

                // Kiểm tra lỗi validation trước khi lưu thay đổi
                var validationErrors = db.GetValidationErrors();
                if (validationErrors.Any())
                {
                    throw new DbEntityValidationException("Validation failed for one or more entities.", validationErrors);
                }

                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property: {error.PropertyName} Error: {error.ErrorMessage}");
                    }
                }
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi cập nhật mật khẩu. Vui lòng kiểm tra lại thông tin.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi cập nhật mật khẩu: " + ex.Message;
                return View();
            }

            // Gửi email cho người dùng
            string emailBody = $"Xin chào {khachHang.TenKH},\n\nMật khẩu mới của bạn là: {newPassword}\nHãy đăng nhập và thay đổi mật khẩu ngay lập tức.";
            SendEmail(khachHang.Email, "Khôi phục mật khẩu", emailBody);
            ViewBag.SuccessMessage = "Mật khẩu mới đã được gửi vào email của bạn!";
            return View();
        }
        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("crandi21112004@gmail.com", "Võ Duy Ân - HUFLIT");
            var toAddress = new MailAddress(toEmail);
            string fromPassword = "wkdo vwwt ufkk kmgh"; // Sử dụng App Password

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}