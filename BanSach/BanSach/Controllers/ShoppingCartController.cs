using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace BanSach.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly db_Book db = new db_Book();

        // Tính tổng tiền đơn hàng
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult BagCart()
        {
            int total_quantity_item = 0;
            if (Session["Cart"] is Cart cart)
            {
                total_quantity_item = cart.Total_quantity();
            }
            ViewBag.QuantityCart = total_quantity_item;
            return PartialView("BagCart");
        }

        // GET: ShoppingCart
        public ActionResult ShowCart()
        {
            if (Session["Cart"] == null)
                return View("ShowCart");
            Cart _cart = Session["Cart"] as Cart;
            return View(_cart);
        }

        // Tạo mới giỏ hàng
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }

        [HttpGet]
        public ActionResult AddToCart(int id)
        {
            try
            {
                var _pro = db.SanPham.Include(s => s.KhuyenMai).SingleOrDefault(s => s.IDsp == id);
                if (_pro != null)
                {
                    decimal mucGiamGia = _pro.KhuyenMai != null ? _pro.KhuyenMai.MucGiamGia ?? 0 : 0;
                    GetCart().Add_Product_Cart(_pro, mucGiamGia);
                    return Json(new { success = true, message = $"Thêm {_pro.TenSP} thành công", productName = _pro.TenSP }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult MuaNgay(int id)
        {
            var _pro = db.SanPham.Include(s => s.KhuyenMai).SingleOrDefault(s => s.IDsp == id);
            if (_pro != null)
            {
                decimal mucGiamGia = _pro.KhuyenMai != null ? _pro.KhuyenMai.MucGiamGia ?? 0 : 0;
                GetCart().Add_Product_Cart(_pro, mucGiamGia);
            }
            return RedirectToAction("ShowCart");
        }

        public ActionResult Update_Cart_Quantity()
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = int.Parse(Request.Form["idPro"]);
            int _quantity = int.Parse(Request.Form["carQuantity"]);
            cart.Update_quantity(id_pro, _quantity);
            return RedirectToAction("ShowCart");
        }

        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_CartItem(id);
            return RedirectToAction("ShowCart");
        }

        public ActionResult CheckOut(FormCollection form)
        {
            try
            {
                Cart cart = Session["Cart"] as Cart;
                if (cart == null || !cart.Items.Any())
                {
                    return RedirectToAction("ShowCart");
                }

                string address = form["AddressDeliverry"];
                string codeCustomer = form["CodeCustomer"];
                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(codeCustomer))
                {
                    return Content("Vui lòng điền đầy đủ thông tin giao hàng và mã khách hàng.");
                }

                DonHang _order = new DonHang
                {
                    NgayDatHang = DateTime.Now,
                    DiaChi = address,
                    IDkh = int.Parse(codeCustomer),
                    TrangThai = "Chờ xử lý",
                    totalamount = cart.Items.Sum(item => item._product.GiaBan * (1 - item.MucGiamGia / 100) * item._quantity)
                };
                db.DonHang.Add(_order);
                db.SaveChanges();

                foreach (var item in cart.Items)
                {
                    var giaBan = item._product.GiaBan;
                    var mucGiamGia = item.MucGiamGia;
                    var giaSauKhuyenMai = giaBan * (1 - (mucGiamGia / 100));

                    var _order_detail = new DonHangCT
                    {
                        IDDonHang = _order.IDdh,
                        IDSanPham = item._product.IDsp,
                        Gia = (double)giaSauKhuyenMai,
                        SoLuong = item._quantity
                    };
                    db.DonHangCT.Add(_order_detail);

                    var product = db.SanPham.SingleOrDefault(s => s.IDsp == item._product.IDsp);
                    if (product != null)
                    {
                        product.SoLuong -= item._quantity;
                    }
                }
                db.SaveChanges();

                Session["OrderId"] = _order.IDdh;

                decimal totalAmount = cart.Items.Sum(item => item._product.GiaBan * (1 - item.MucGiamGia / 100) * item._quantity);
                string payUrl = CreateZaloPayPayment(_order.IDdh, totalAmount); // Thay bằng ZaloPay
                if (!string.IsNullOrEmpty(payUrl))
                {
                    return Redirect(payUrl);
                }
                return Content("Không thể tạo yêu cầu thanh toán ZaloPay.");
            }
            catch (Exception ex)
            {
                return Content($"Có sai sót! Xin kiểm tra lại thông tin: {ex.Message}");
            }
        }

        private string CreateZaloPayPayment(int orderId, decimal amount)
        {
            // Thay bằng thông tin từ ZaloPay Sandbox của bạn
            string appId = "2553"; // Lấy từ ZaloPay Developer
            string key1 = "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";    // Lấy từ ZaloPay Developer
            string apiEndpoint = "https://sb-openapi.zalopay.vn/v2/create";
            string returnUrl = "https://intent-admittedly-redbird.ngrok-free.app/ShoppingCart/ZaloPayReturn";
            long requestAmount = (long)(amount * 1000);
            if (requestAmount < 2000)
            {
                System.Diagnostics.Debug.WriteLine($"Amount {requestAmount} nhỏ hơn mức tối thiểu 2000 VND.");
                return null;
            }

            string appTransId = $"{DateTime.Now:yyMMdd}_{orderId}";
            long appTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            string appUser = "user" + orderId;
            string orderInfo = $"Thanh toán đơn hàng #{orderId}";
            string embedData = JsonConvert.SerializeObject(new { redirecturl = returnUrl });

            var request = new Dictionary<string, string>
    {
        { "appid", appId },
        { "apptransid", appTransId },
        { "apptime", appTime.ToString() },
        { "appuser", appUser },
        { "amount", requestAmount.ToString() },
        { "item", JsonConvert.SerializeObject(new { orderId }) },
        { "description", orderInfo },
        { "embeddata", embedData }
    };

            string data = $"{request["appid"]}|{request["apptransid"]}|{request["appuser"]}|{request["amount"]}|{request["apptime"]}|{request["embeddata"]}|{request["description"]}";
            string mac = GetHMACSHA256(data, key1);
            request["mac"] = mac;

            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(request);
                var response = client.PostAsync(apiEndpoint, content).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;

                System.Diagnostics.Debug.WriteLine($"ZaloPay Request: {JsonConvert.SerializeObject(request)}");
                System.Diagnostics.Debug.WriteLine($"ZaloPay Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"ZaloPay Response: {responseString}");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
                    if (responseData.return_code == 1)
                    {
                        return responseData.order_url.ToString();
                    }
                    System.Diagnostics.Debug.WriteLine($"ZaloPay lỗi: {responseData.return_message}");
                    return null;
                }
                return null;
            }
        }

        [HttpPost]
        public ActionResult ZaloPayNotify()
        {
            string data = Request.Form["data"];
            string mac = Request.Form["mac"];
            string key2 = "eG4r0GcoNtRGbO8"; // Thay bằng Key2 từ ZaloPay Sandbox

            if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(mac))
            {
                string computedMac = GetHMACSHA256(data, key2);
                if (computedMac == mac)
                {
                    var jsonData = JsonConvert.DeserializeObject<dynamic>(data);
                    string appTransId = jsonData.apptransid;
                    if (appTransId != null)
                    {
                        string orderId = appTransId.ToString().Split('_')[1];
                        var order = db.DonHang.Find(int.Parse(orderId));
                        if (order != null && order.TrangThai != "Paid")
                        {
                            order.TrangThai = "Paid";
                            db.SaveChanges();
                        }
                    }
                }
            }
            return new HttpStatusCodeResult(200);
        }

        public ActionResult ZaloPayReturn()
        {
            // ZaloPay redirect về đây sau khi thanh toán
            // Thực tế cần kiểm tra trạng thái qua API Query, nhưng để đơn giản ta giả định thành công từ returnUrl
            string appTransId = Request.QueryString["apptransid"];
            if (!string.IsNullOrEmpty(appTransId))
            {
                string orderId = appTransId.Split('_')[1]; // Lấy orderId từ apptransid
                var order = db.DonHang.Find(int.Parse(orderId));
                if (order != null)
                {
                    order.TrangThai = "Paid";
                    db.SaveChanges();
                    Session["OrderId"] = order.IDdh;
                    Cart cart = Session["Cart"] as Cart;
                    cart?.ClearCart();
                    return RedirectToAction("CheckOut_Success");
                }
            }
            TempData["ErrorMessage"] = "Thanh toán thất bại.";
            return RedirectToAction("ShowCart");
        }

        

        public ActionResult CheckOut_Success()
        {
            if (Session["OrderId"] == null)
            {
                return RedirectToAction("ShowCart");
            }

            var orderId = (int)Session["OrderId"];
            var order = db.DonHang.Find(orderId);
            if (order != null)
            {
                ViewBag.OrderId = order.IDdh;
                ViewBag.TotalAmount = order.Total_DH;
            }
            return View();
        }

        private string GetHMACSHA256(string data, string key)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("Dữ liệu hoặc khóa bí mật không được để trống.");
            }

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}