using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
                    // Lưu dòng sản phẩm vào chi tiết hóa đơn
                    DonHangCT _order_detail = new DonHangCT();
                    _order_detail.IDDonHang = _order.IDdh;
                    _order_detail.IDSanPham = item._product.IDsp;

                    // Tính giá sau khi đã áp dụng khuyến mãi
                    decimal giaBan = item._product.GiaBan;
                    decimal mucGiamGia = item.MucGiamGia;
                    decimal giaSauKhuyenMai = giaBan * (1 - (mucGiamGia / 100));

                    _order_detail.Gia = (double)giaSauKhuyenMai; // Lưu giá sau khi đã áp dụng khuyến mãi
                    _order_detail.SoLuong = item._quantity;

                    db.DonHangCT.Add(_order_detail);

                    // Xử lý cập nhật lại số lượng tồn trong bảng SanPham
                    var product = db.SanPham.SingleOrDefault(s => s.IDsp == item._product.IDsp);
                    if (product != null)
                    {
                        product.SoLuong -= item._quantity; // Số lượng tồn mới = số lượng tồn - số lượng đã mua
                    }
                }

                db.SaveChanges();
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
            return View();
        }
     
    }
}