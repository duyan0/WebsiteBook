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

            // Kiểm tra trùng lặp tài khoản, số điện thoại, và email
            // Kiểm tra tài khoản đã tồn tại trong bảng KhachHang hoặc Admin
            bool isAccountExist = db.KhachHang.Any(s => s.TKhoan == _user.TKhoan);

            // Kiểm tra số điện thoại đã tồn tại trong bảng KhachHang
            bool isPhoneExist = db.KhachHang.Any(s => s.SoDT == _user.SoDT);

            // Kiểm tra email đã tồn tại trong bảng KhachHang
            bool isEmailExist = db.KhachHang.Any(s => s.Email == _user.Email);

            // Nếu có bất kỳ thông tin nào tồn tại thì báo lỗi
            if (isAccountExist || isPhoneExist || isEmailExist)
            {
                if (isAccountExist)
                {
                    ViewBag.ErrorRegister = "Tài khoản đã tồn tại!";
                }
                else if (isPhoneExist)
                {
                    ViewBag.ErrorRegister = "Số điện thoại đã tồn tại!";
                }
                else if (isEmailExist)
                {
                    ViewBag.ErrorRegister = "Email đã tồn tại!";
                }

                return View();
            }

            // Kiểm tra mật khẩu khớp nhau
            if (_user.MKhau != ConfirmPass)
            {
                ViewBag.ErrorRegister = "Mật khẩu nhập lại không đúng!";
                return View();
            }

            // Lưu người dùng vào cơ sở dữ liệu mà không có OTP ngay lập tức
            _user.TrangThaiTaiKhoan = "Chưa xác nhận"; // Đặt trạng thái ban đầu là chưa xác nhận
            db.KhachHang.Add(_user);
            db.SaveChanges(); // Lưu người dùng mà không có OTP

            // Tạo mã OTP ngẫu nhiên (4 chữ số)
            string otp = GenerateOtp();
            _user.OTP = otp;
            _user.OTPExpiry = DateTime.Now.AddMinutes(5); // Mã OTP có hiệu lực trong 5 phút

            db.SaveChanges(); // Cập nhật mã OTP vào cơ sở dữ liệu

            // Gửi email chứa mã OTP
            SendEmail(_user.Email, "Xác nhận tài khoản", $"Vui lòng xác nhận tài khoản của bạn bằng cách nhập mã OTP: {otp}");

            // Chuyển hướng đến trang nhập mã OTP
            return RedirectToAction("VerifyOtp", new { userId = _user.IDkh });
        }


        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(1000, 9999).ToString(); // Tạo mã OTP 4 chữ số
        }
        [HttpGet]
        public ActionResult VerifyOtp(int userId)
        {
            var user = db.KhachHang.FirstOrDefault(u => u.IDkh == userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy người dùng.";
                return View();
            }

            return View(new VerifyOtpViewModel { UserId = userId });
        }

        [HttpPost]
        public ActionResult VerifyOtp(VerifyOtpViewModel model)
        {
            var user = db.KhachHang.FirstOrDefault(u => u.IDkh == model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy người dùng.";
                return View();
            }

            // Kiểm tra mã OTP có hợp lệ không
            if (user.OTP != model.EnteredOtp)
            {
                ViewBag.ErrorMessage = "Mã OTP không đúng.";
                return View(model);
            }

            // Kiểm tra mã OTP có hết hạn không
            if (user.OTPExpiry?.AddMinutes(5) < DateTime.Now)
            {
                ViewBag.ErrorMessage = "Mã OTP đã hết hạn.";
                return View(model);
            }

            // Cập nhật trạng thái tài khoản thành đã xác nhận
            user.TrangThaiTaiKhoan = "Hoạt động";
            user.OTP = null;  // Xóa mã OTP đã sử dụng
            db.SaveChanges();

            ViewBag.Message = "Email đã được xác nhận thành công!";
            return RedirectToAction("LoginAccountCus", "LoginUser");
        }
        [HttpGet]
        public ActionResult VerifyEmail(int userId)
        {
            var user = db.KhachHang.FirstOrDefault(u => u.IDkh == userId);
            if (user != null)
            {
                user.TrangThaiTaiKhoan = "Hoạt động";
                db.SaveChanges();
                ViewBag.Message = "Email đã được xác nhận thành công!";
            }
            else
            {
                ViewBag.Message = "Không tìm thấy người dùng.";
            }

            return View();
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
            string fromPassword = "uisz jzid byry jtqw"; // Sử dụng App Password

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

        public ActionResult TestEmail()
        {
            SendEmail("crandi21112004@gmail.com", "Test Email", "This is a test email.");
            return Content("Email sent!");
        }
    }
}