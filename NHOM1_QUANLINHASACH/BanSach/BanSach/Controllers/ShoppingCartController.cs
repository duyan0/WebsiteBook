using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class ShoppingCartController : Controller
    {
        private dbSach db = new dbSach();

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
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        // Thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id)
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
            return RedirectToAction("TrangSP", "SanPhams", new { id });
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

                cart.ClearCart();
                return RedirectToAction("CheckOut_Success", "ShoppingCart");

            }
            catch
            {
                return Content("Có sai sót! Xin kiểm tra lại thông tin");
            }
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


        public ActionResult VNPAYPayment(int orderId)
        {
            var order = db.DonHang.Find(orderId);  // Lấy đơn hàng từ cơ sở dữ liệu
            if (order == null)
            {
                return RedirectToAction("ShowCart", "ShoppingCart");
            }

            var vnp = new VNPAYClient();  // Tạo đối tượng VNPAYClient
            string vnpayUrl = vnp.CreatePaymentRequest(order);
            Console.WriteLine("VNPAY URL: " + vnpayUrl);  // Kiểm tra URL tạo ra có đúng không
            if (string.IsNullOrEmpty(vnpayUrl))
            {
                return Content("Có lỗi khi tạo URL thanh toán VNPAY.");
            }
            return Redirect(vnpayUrl);
        }



        public ActionResult VNPAYCallback(FormCollection form)
        {
            string vnp_SecureHash = form["vnp_SecureHash"];
            string vnp_SecureHashValidate = VNPAYClient.GetSecureHash(form.ToString());

            if (vnp_SecureHash == vnp_SecureHashValidate)
            {
                // Kiểm tra trạng thái thanh toán từ VNPAY
                if (form["vnp_ResponseCode"] == "00")
                {
                    // Thanh toán thành công
                    int orderId = int.Parse(form["vnp_TxnRef"]);
                    var order = db.DonHang.Find(orderId);
                    if (order != null)
                    {
                        order.TrangThai = "Paid";  // Cập nhật trạng thái thanh toán
                        db.SaveChanges();
                    }

                    // Truyền thông tin đơn hàng đến View
                    ViewBag.OrderId = form["vnp_TxnRef"];
                    ViewBag.TotalAmount = (decimal.Parse(form["vnp_Amount"]) / 100);  // VNPAY trả lại số tiền tính bằng đồng

                    return View("CheckOut_Success");
                }
                else
                {
                    // Thanh toán thất bại
                    return RedirectToAction("CheckOut_Failed", "ShoppingCart");
                }
            }
            else
            {
                // Mã bảo mật không hợp lệ
                return RedirectToAction("CheckOut_Failed", "ShoppingCart");
            }
        }
    }
}