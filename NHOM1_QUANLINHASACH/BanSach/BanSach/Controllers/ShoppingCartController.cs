using BanSach.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace BanSach.Controllers
{
    public class ShoppingCartController : Controller
    {
        private static readonly string PartnerCode = "MOMO";
        private static readonly string AccessKey = "F8BBA842ECF85";
        private static readonly string SecretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
        private static readonly string ApiEndpoint = "https://test-payment.momo.vn";
        private readonly dbSach db = new dbSach();

        // Tính tổng tiền đơn hàng
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult BagCart()
        {
            int total_quantity_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
                total_quantity_item = cart.Total_quantity();
            ViewBag.QuantityCart = total_quantity_item;
            return PartialView("BagCart");
        }

        // GET: ShoppingCart, chuẩn bị dữ liệu cho View
        public ActionResult ShowCart()
        {
            if (Session["Cart"] == null)
                return View("ShowCart");
            Cart _cart = Session["Cart"] as Cart;
            return View(_cart);
        }
        // Tạo mới giỏ hàng, nguồn được lấy từ Session
        public Cart GetCart()
        {
            if (Session["Cart"] == null)
            {
                Cart cart = new Cart();
                Session["Cart"] = cart;
            }
            return (Cart)Session["Cart"];
        }

        // Thêm sản phẩm vào giỏ hàng
        // Giả sử bạn có đối tượng Cart để lưu thông tin giỏ hàng
        public ActionResult AddToCart(int productId, int quantity)
        {
            var product = db.SanPham.Find(productId); // Lấy sản phẩm từ cơ sở dữ liệu
            if (product != null)
            {
                // Kiểm tra giỏ hàng trong session
                Cart cart = Session["Cart"] as Cart;
                if (cart == null)
                {
                    cart = new Cart(); // Tạo mới giỏ hàng nếu chưa có
                }

                // Thêm sản phẩm vào giỏ
                cart.Add_Product_Cart(product, 0, quantity); // 0 là mức giảm giá mặc định
                Session["Cart"] = cart; // Lưu giỏ hàng vào session
            }

            return RedirectToAction("Cart");
        }

        //Mua ngay chuyển tới giỏ hàng
        public ActionResult MuaNgay(int id)
        {
            // Lấy sản phẩm theo id, đồng thời lấy cả thông tin khuyến mãi
            var _pro = db.SanPham.Include(s => s.KhuyenMai).SingleOrDefault(s => s.IDsp == id);
            if (_pro != null)
            {
                // Kiểm tra xem sản phẩm có khuyến mãi không
                decimal mucGiamGia = _pro.KhuyenMai != null ? _pro.KhuyenMai.MucGiamGia ?? 0 : 0;

                // Thêm sản phẩm vào giỏ hàng cùng với mức giảm giá
                GetCart().Add_Product_Cart(_pro, mucGiamGia);
            }
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        // Cập nhật số lượng và tính lại tổng tiền
        public ActionResult Update_Cart_Quantity(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = int.Parse(Request.Form["idPro"]);
            int _quantity = int.Parse(Request.Form["carQuantity"]);
            cart.Update_quantity(id_pro, _quantity);

            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        // Xóa dòng sản phẩm trong giỏ hàng
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_CartItem(id);

            return RedirectToAction("ShowCart", "ShoppingCart");
        }

        public ActionResult CheckOut(FormCollection form)
        {
            try
            {
                Cart cart = Session["Cart"] as Cart;

                if (cart == null || !cart.Items.Any())
                {
                    return RedirectToAction("ShowCart", "ShoppingCart");
                }

                DonHang _order = new DonHang();
                _order.NgayDatHang = DateTime.Now;
                _order.DiaChi = form["AddressDeliverry"];
                _order.IDkh = int.Parse(form["CodeCustomer"]);

                db.DonHang.Add(_order);

                foreach (var item in cart.Items)
                {
                    DonHangCT _order_detail = new DonHangCT();
                    _order_detail.IDDonHang = _order.IDdh;
                    _order_detail.IDSanPham = item._product.IDsp;

                    decimal giaBan = item._product.GiaBan;
                    decimal mucGiamGia = item.MucGiamGia;
                    decimal giaSauKhuyenMai = giaBan * (1 - (mucGiamGia / 100));

                    _order_detail.Gia = (double)giaSauKhuyenMai;
                    _order_detail.SoLuong = item._quantity;

                    db.DonHangCT.Add(_order_detail);

                    var product = db.SanPham.SingleOrDefault(s => s.IDsp == item._product.IDsp);
                    if (product != null)
                    {
                        product.SoLuong -= item._quantity;
                    }
                }

                db.SaveChanges();

                // Lưu OrderId vào Session sau khi tạo đơn hàng thành công
                Session["OrderId"] = _order.IDdh;

                // Gọi phương thức gửi email
                SendOrderConfirmationEmail(_order);

                cart.ClearCart();

                return RedirectToAction("CheckOut_Success", "ShoppingCart", new { orderId = _order.IDdh });

            }
            catch
            {
                return RedirectToAction("CheckOut_Success", "ShoppingCart");
            }
        }




        public ActionResult CheckOut_Failed ()
        {
            return View();
        }

        // Thông báo thanh toán thành công
        public ActionResult CheckOut_Success()
        {
            if (Session["OrderId"] == null)
            {
                return RedirectToAction("ShowCart", "ShoppingCart");
            }

            var orderId = (int)Session["OrderId"];
            var order = db.DonHang.Find(orderId);
            if (order != null)
            {
                ViewBag.OrderId = order.IDdh;
                ViewBag.TotalAmount = order.TongTien;
            }

            return View();

        }
        public ActionResult MoMoCheckout(int orderId)
        {
            if (orderId == 0)
            {
                return RedirectToAction("ErrorPage"); // Chuyển hướng về trang lỗi nếu orderId không hợp lệ
            }

            var order = db.DonHang.Find(orderId);
            if (order == null)
            {
                return RedirectToAction("ShowCart"); // Nếu không tìm thấy đơn hàng, chuyển đến trang giỏ hàng
            }

            var amount = order.TongTien.ToString();
            var orderInfo = $"Thanh toán đơn hàng #{orderId}";
            var orderIdMoMo = $"ORDER{orderId}{DateTime.Now.Ticks}"; // Tạo ID đơn hàng đặc biệt cho MoMo

            // Tạo tham số request gửi đến MoMo API
            var request = new
            {
                partnerCode = PartnerCode,
                accessKey = AccessKey,
                requestId = orderIdMoMo,
                amount,
                orderId = orderIdMoMo,  // Đây là ID đơn hàng của MoMo, không phải ID từ database
                orderInfo,
                returnUrl = "https://localhost:44307/ShoppingCart/CheckOut_Success", // Địa chỉ trả về sau thanh toán
                notifyUrl = "https://localhost:44307/ShoppingCart/CheckOut_Success", // Địa chỉ thông báo kết quả thanh toán
                requestType = "payWithMethod" // Loại thanh toán
            };

            // Tạo chữ ký bảo mật
            var rawHash = $"{request.partnerCode}&{request.accessKey}&{request.requestId}&{request.amount}&{request.orderId}&{request.orderInfo}&{request.returnUrl}&{request.notifyUrl}";
            var signature = CreateSignature(rawHash); // Tạo chữ ký bảo mật bằng hàm CreateSignature

            // Thêm chữ ký vào request
            var payload = new
            {
                request.partnerCode,
                request.accessKey,
                request.requestId,
                request.amount,
                request.orderId,
                request.orderInfo,
                request.returnUrl,
                request.notifyUrl,
                signature,
                request.requestType
            };

            // Gửi yêu cầu thanh toán đến MoMo API
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = client.PostAsync(ApiEndpoint, content).Result;  // Gửi yêu cầu POST

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var resultJson = JsonConvert.DeserializeObject<dynamic>(result);

                if (resultJson?.resultCode == "0") // Kiểm tra mã trả về từ MoMo API
                {
                    var paymentUrl = resultJson.payUrl.ToString(); // URL thanh toán từ MoMo
                    return Redirect(paymentUrl); // Chuyển hướng người dùng đến MoMo để thanh toán
                }
                else
                {
                    string errorCode = resultJson?.resultCode ?? "Unknown";
                    string errorMessage = resultJson?.message ?? "Unknown error";
                    return Content($"Đã có lỗi xảy ra trong quá trình thanh toán. Mã lỗi: {errorCode}, Thông báo: {errorMessage}");
                }
            }
            else
            {
                return Content("Lỗi kết nối với MoMo. Vui lòng thử lại sau.");
            }
        }


        private string CreateSignature(string rawHash)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }


        // MoMo Notify (receiving callback)
        public ActionResult MoMoNotify()
        {
            try
            {
                var resultData = Request.Form.ToString();
                var signature = Request.Form["signature"];

                // Check if signature is valid
                var rawHash = Request.Form.ToString(); // Dữ liệu thô cần mã hóa lại
                var checkSignature = CreateSignature(rawHash);

                if (checkSignature == signature)
                {
                    var orderId = Request.Form["orderId"];
                    var resultCode = Request.Form["resultCode"];

                    if (resultCode == "0") // Thanh toán thành công
                    {
                        var order = db.DonHang.Find(orderId);
                        if (order != null)
                        {
                            order.TrangThai = "Đã thanh toán";
                            db.SaveChanges();
                        }
                        return RedirectToAction("CheckOut_Success", "ShoppingCart");
                    }
                    else
                    {
                        return Content($"Thanh toán thất bại! Mã lỗi: {resultCode}");
                    }
                }
                else
                {
                    return Content("Chữ ký không hợp lệ!");
                }

            }
            catch (Exception ex)
            {
                return Content($"Có lỗi xảy ra: {ex.Message}");
            }
        }
        public void SendOrderConfirmationEmail(DonHang order)
        {
            try
            {
                string subject = $"Xác nhận đơn hàng #{order.IDdh}";
                string body = $"Chào bạn {order.KhachHang.TenKH},<br><br>" +
                              $"Cảm ơn bạn đã đặt hàng tại cửa hàng chúng tôi. Dưới đây là thông tin đơn hàng của bạn:<br>" +
                              $"<strong>Đơn hàng ID:</strong> {order.IDdh}<br>" +
                              $"<strong>Ngày đặt:</strong> {order.NgayDatHang}<br>" +
                              $"<strong>Địa chỉ giao hàng:</strong> {order.DiaChi}<br>" +
                              $"<strong>Tổng tiền:</strong> {order.TongTien:C}<br><br>" +
                              "Chúng tôi sẽ gửi thông tin cập nhật đơn hàng khi có tiến trình mới. Cảm ơn bạn đã tin tưởng sử dụng dịch vụ của chúng tôi.";

                var message = new MailMessage();
                message.To.Add(new MailAddress(order.KhachHang.Email)); 
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.From = new MailAddress("crandi21112004@gmail.com"); // Địa chỉ email gửi

                // Gửi email
                using (var smtp = new SmtpClient("smtp.gmail.com", 587)) // Gmail SMTP server
                {
                    smtp.EnableSsl = true; // Bật SSL
                    smtp.Credentials = new System.Net.NetworkCredential("crandi21112004@gmail.com", "uiszjzidbyryjtqw"); // Mật khẩu ứng dụng
                    smtp.Send(message); // Gửi email
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Lỗi khi gửi email: " + ex.Message);
            }

        }

    }
}