using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayPal.Api;
using System.Configuration;

namespace BanSach.Controllers
{
    public class OnlinePaymentController : Controller
    {
        private readonly db_Book db = new db_Book();

        // Thông tin PayPal (thay bằng giá trị thật từ PayPal Developer)
        private readonly string PayPalClientId = "Af7pzH3tSDJ90z8q3NCEp8S_-qz1XrG0ilbufIzntSnOWxIuBCvrjh9GeEyaprlaKD39FNqWCgHAit0Y"; // Thay bằng Client ID
        private readonly string PayPalSecret = "EPaQU1JIsq2vB_0qQpEanff_WeJNwP8ZAg8AchvSxnN1wrope8LsVqyJH_lOOXWYPkpfxc-rDVbzjvaq"; // Thay bằng Secret
        private readonly string PayPalMode = "sandbox"; // Hoặc "live" khi triển khai thật
        private readonly string BaseUrl = "https://localhost:44307"; // Thay bằng domain của bạn

        private Payment payment;

        // Lấy API Context cho PayPal
        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
            {
                { "mode", PayPalMode },
                { "clientId", PayPalClientId },
                { "clientSecret", PayPalSecret }
            };
            var accessToken = new OAuthTokenCredential(PayPalClientId, PayPalSecret, config).GetAccessToken();
            return new APIContext(accessToken) { Config = config };
        }

        [HttpPost]
        public ActionResult CreatePayPalPayment(string CodeCustomer, string NameCustomer, string PhoneCustomer, string AddressDelivery)
        {
            if (string.IsNullOrEmpty(CodeCustomer))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập trước khi thanh toán.";
                return RedirectToAction("ShowCart", "ShoppingCart");
            }

            Cart cart = Session["Cart"] as Cart;
            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống.";
                return RedirectToAction("ShowCart", "ShoppingCart");
            }

            try
            {
                // Kiểm tra nếu chưa có key PayPal
                if (PayPalClientId == "YOUR_PAYPAL_CLIENT_ID" || PayPalSecret == "YOUR_PAYPAL_SECRET")
                {
                    TempData["ErrorMessage"] = "Chưa cấu hình PayPal Client ID hoặc Secret. Vui lòng đăng ký tại PayPal Developer.";
                    return RedirectToAction("ShowCart", "ShoppingCart");
                }

                // Tạo đơn hàng
                DonHang donHang = new DonHang
                {
                    IDkh = int.Parse(CodeCustomer),
                    NgayDatHang = DateTime.Now,
                    TongTien = cart.Total_money(),
                    DiaChi = AddressDelivery,
                    TrangThai = "Chờ thanh toán",
                    State = new PendingState()
                };
                db.DonHang.Add(donHang);
                db.SaveChanges();

                // Tạo chi tiết đơn hàng
                foreach (var item in cart.Items)
                {
                    DonHangCT donHangCT = new DonHangCT
                    {
                        IDDonHang = donHang.IDdh,
                        IDSanPham = item._product.IDsp,
                        SoLuong = item._quantity,
                        Gia = item._product.GiaBan
                    };
                    db.DonHangCT.Add(donHangCT);
                }
                db.SaveChanges();

                // Tạo thanh toán PayPal
                var apiContext = GetAPIContext();
                var totalAmount = cart.Total_money().ToString("0.00"); // USD, định dạng 2 chữ số thập phân
                var orderId = donHang.IDdh.ToString();

                var itemList = new ItemList
                {
                    items = cart.Items.Select(item => new Item
                    {
                        name = item._product.TenSP,
                        currency = "USD",
                        price = (item._product.GiaBan * (1 - (item.MucGiamGia / 100))).ToString("0.00"),
                        quantity = item._quantity.ToString(),
                        sku = item._product.IDsp.ToString()
                    }).ToList()
                };

                var transaction = new Transaction
                {
                    amount = new Amount
                    {
                        currency = "USD",
                        total = totalAmount,
                        details = new Details
                        {
                            subtotal = totalAmount
                        }
                    },
                    description = $"Thanh toán đơn hàng #{orderId}",
                    item_list = itemList,
                    invoice_number = orderId
                };

                payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction> { transaction },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = $"{BaseUrl}/OnlinePayment/PayPalSuccess?orderId={orderId}",
                        cancel_url = $"{BaseUrl}/OnlinePayment/PayPalCancel?orderId={orderId}"
                    }
                };

                var createdPayment = payment.Create(apiContext);

                // Lấy URL thanh toán
                var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;
                if (string.IsNullOrEmpty(approvalUrl))
                {
                    throw new Exception("Không thể tạo URL thanh toán PayPal.");
                }

                return Redirect(approvalUrl);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo thanh toán PayPal: " + ex.Message;
                return RedirectToAction("ShowCart", "ShoppingCart");
            }
        }

        public ActionResult PayPalSuccess(string orderId, string paymentId, string token, string PayerID)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    TempData["ErrorMessage"] = "Thiếu thông tin đơn hàng.";
                    return RedirectToAction("ShowCart", "ShoppingCart");
                }

                // Tìm đơn hàng
                int donHangId = int.Parse(orderId);
                var donHang = db.DonHang.Find(donHangId);
                if (donHang == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                    return RedirectToAction("ShowCart", "ShoppingCart");
                }

                // Thực thi thanh toán
                var apiContext = GetAPIContext();
                var paymentExecution = new PaymentExecution { payer_id = PayerID };
                payment = new Payment { id = paymentId };

                var executedPayment = payment.Execute(apiContext, paymentExecution);

                if (executedPayment.state.ToLower() != "approved")
                {
                    TempData["ErrorMessage"] = "Thanh toán không được chấp thuận.";
                    return RedirectToAction("ShowCart", "ShoppingCart");
                }

                // Cập nhật trạng thái đơn hàng
                donHang.TrangThai = "Chờ xử lý";
                donHang.State = new PendingState();

                // Cập nhật số lượng sản phẩm
                var cart = Session["Cart"] as Cart;
                foreach (var item in cart.Items)
                {
                    var product = db.SanPham.Find(item._product.IDsp);
                    product.SoLuong -= item._quantity;
                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                }

                db.SaveChanges();

                // Xóa giỏ hàng
                cart.ClearCart();
                Session["Cart"] = cart;

                TempData["SuccessMessage"] = $"Thanh toán PayPal thành công - Mã đơn hàng - {orderId}";
                return RedirectToAction("lichsudonhang", "khachhangs");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xử lý thanh toán PayPal: " + ex.Message;
                return RedirectToAction("ShowCart", "ShoppingCart");
            }
        }

        public ActionResult PayPalCancel(string orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    TempData["ErrorMessage"] = "Thiếu thông tin đơn hàng.";
                    return RedirectToAction("ShowCart", "ShoppingCart");
                }

                // Tìm và xóa đơn hàng
                int donHangId = int.Parse(orderId);
                var donHang = db.DonHang.Find(donHangId);
                if (donHang != null)
                {
                    db.DonHangCT.RemoveRange(db.DonHangCT.Where(ct => ct.IDDonHang == donHangId));
                    db.DonHang.Remove(donHang);
                    db.SaveChanges();
                }

                TempData["ErrorMessage"] = "Thanh toán PayPal đã bị hủy.";
                return RedirectToAction("ShowCart", "ShoppingCart");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi hủy thanh toán: " + ex.Message;
                return RedirectToAction("ShowCart", "ShoppingCart");
            }
        }
    }
}