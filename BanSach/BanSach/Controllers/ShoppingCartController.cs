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
        [HttpGet]
        public ActionResult GetCartDetails(int id)
        {
            try
            {
                Cart cart = GetCart(); // Lấy giỏ hàng hiện tại
                var _pro = db.SanPham.Include(s => s.KhuyenMai).SingleOrDefault(s => s.IDsp == id);
                if (_pro != null)
                {
                    decimal mucGiamGia = _pro.KhuyenMai != null ? _pro.KhuyenMai.MucGiamGia ?? 0 : 0;
                    cart.Add_Product_Cart(_pro, mucGiamGia); // Thêm sản phẩm vào giỏ hàng

                    // Tạo dữ liệu trả về
                    var cartDetails = new
                    {
                        success = true,
                        message = $"Thêm {_pro.TenSP} thành công",
                        productName = _pro.TenSP,
                        totalQuantity = cart.Total_quantity(), // Tổng số lượng sản phẩm trong giỏ
                        totalMoney = cart.Total_money(), // Tổng tiền (nếu có phương thức này trong Cart)
                        items = cart.Items.Select(item => new
                        {
                            productId = item._product.IDsp,
                            productName = item._product.TenSP,
                            quantity = item._quantity,
                            price = item._product.GiaBan * (1 - (item.MucGiamGia / 100)),
                            image = item._product.HinhAnh
                        }).ToList()
                    };

                    return Json(cartDetails, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
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
        //Mua ngay chosen tới giỏ hàng
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
        public ActionResult Update_Cart_Quantity()
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

        [HttpPost]
        public ActionResult CheckOut(string CodeCustomer, string NameCustomer, string PhoneCustomer, string AddressDeliverry)
        {
            if (string.IsNullOrEmpty(CodeCustomer))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập trước khi đặt hàng.";
                return RedirectToAction("Cart");
            }

            Cart cart = Session["Cart"] as Cart;


            try
            {
                // Tạo đơn hàng
                DonHang donHang = new DonHang
                {
                    IDkh = int.Parse(CodeCustomer),
                    NgayDatHang = DateTime.Now,
                    TongTien = cart.Total_money(),
                    DiaChi = AddressDeliverry,
                    TrangThai = "Chờ xử lý",
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

                    // Cập nhật số lượng sản phẩm
                    var product = db.SanPham.Find(item._product.IDsp);
                    product.SoLuong -= item._quantity;
                    db.Entry(product).State = EntityState.Modified;
                }
                db.SaveChanges();

                // Xóa giỏ hàng sau khi đặt hàng
                cart.ClearCart();
                Session["Cart"] = cart;

                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("LichSuDonHang", "KhachHangs");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi đặt hàng: " + ex.Message;
                return RedirectToAction("Cart");
            }
        }


    }
}