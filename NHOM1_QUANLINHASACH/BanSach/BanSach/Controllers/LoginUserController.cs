using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Data.Entity.Validation;


namespace BanSach.Controllers
{
    public class LoginUserController : Controller
    {
        dbSach db = new dbSach();
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
        public ActionResult RegisterCus(KhachHang _user)
        {
            // Kiểm tra tính hợp lệ của dữ liệu được truyền vào trước
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorRegister = "Vui lòng điền đầy đủ các thông tin.";
                return View();
            }

            // Kiểm tra trùng lặp tài khoản và thông tin
            var check = db.KhachHang.FirstOrDefault(s => s.TKhoan == _user.TKhoan);
            var check1 = db.Admin.FirstOrDefault(s => s.TKhoan == _user.TKhoan);
            var check2 = db.KhachHang.FirstOrDefault(s => s.SoDT == _user.SoDT);
            var check3 = db.KhachHang.FirstOrDefault(s => s.Email == _user.Email);

            // Kiểm tra tài khoản tồn tại
            if (check != null || check1 != null)
            {
                ViewBag.ErrorRegister = "Tài khoản này đã tồn tại!";
                return View();
            }

            // Kiểm tra số điện thoại trùng lặp
            if (check2 != null)
            {
                ViewBag.ErrorRegister = "Số điện thoại đã được đăng ký!";
                return View();
            }

            // Kiểm tra email trùng lặp
            if (check3 != null)
            {
                ViewBag.ErrorRegister = "Email đã được đăng ký!";
                return View();
            }

            // Kiểm tra mật khẩu không được để trống
            if (string.IsNullOrEmpty(_user.MKhau) || string.IsNullOrEmpty(_user.ConfirmPass))
            {
                ViewBag.ErrorRegister = "Mật khẩu và mật khẩu nhập lại không được để trống!";
                return View();
            }

            // Kiểm tra mật khẩu khớp nhau
            if (_user.MKhau.Trim() != _user.ConfirmPass.Trim())
            {
                ViewBag.ErrorRegister = "Mật khẩu nhập lại không đúng!";
                return View();
            }

            // Nếu tất cả điều kiện đều hợp lệ
            db.Configuration.ValidateOnSaveEnabled = false;
            db.KhachHang.Add(_user);
            db.SaveChanges();

            return View("SignUpSuccess");
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


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult LoginAccountCus(KhachHang _cus)
        {
            // check là khách hàng cần tìm
            var check = db.KhachHang.Where(s => s.TKhoan == _cus.TKhoan && s.MKhau == _cus.MKhau).FirstOrDefault();
            var check2 = db.Admin.Where(x => x.TKhoan == _cus.TKhoan && x.MKhau == _cus.MKhau).FirstOrDefault();
            if (_cus.TKhoan == null)
            {
                return View();
            }
            if (check2 != null)
            {
                Session["IDQly"] = check2.ID;
                Session["TenQly"] = check2.HoTen;
                Session["TKQly"] = check2.TKhoan;
                Session["Vaitro"] = check2.VaiTro;
                return RedirectToAction("Index", "Admins");
            }
            if (check == null)  //không có KH
            {
                ViewBag.ErrorInfo = "Sai tài khoản hoặc mật khẩu";
                return View();
            }
            db.Configuration.ValidateOnSaveEnabled = false;
            Session["IDkh"] = check.IDkh;
            Session["MKhau"] = check.MKhau;
            Session["TenKH"] = check.TenKH;
            Session["SoDT"] = check.SoDT;

            return RedirectToAction("SignInSuccess");

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
            try
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
            catch (SmtpException smtpEx)
            {
                // Ghi lại lỗi SMTP
                System.Diagnostics.Debug.WriteLine("SMTP Error: " + smtpEx.Message);
            }
            catch (Exception ex)
            {
                // Ghi lại các lỗi khác
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }
        public ActionResult TestEmail()
        {
            SendEmail("crandi21112004@gmail.com", "Test Email", "This is a test email.");
            return Content("Email sent!");
        }

    }
}