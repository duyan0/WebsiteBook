using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class LoginUserController : Controller
    {
        SachEntities1 db = new SachEntities1();
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
            if(Session["IdQly"] != null)
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
    }


}