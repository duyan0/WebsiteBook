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

        public ActionResult CheckOut(FormCollection form)
        {
            try
            {
                Cart cart = Session["Cart"] as Cart;

                if (cart == null || !cart.Items.Any())
                {
                    return RedirectToAction("ShowCart", "ShoppingCart");
                }

                DonHang _order = new DonHang
                {
                    NgayDatHang = DateTime.Now,
                    DiaChi = form["AddressDeliverry"],
                    IDkh = int.Parse(form["CodeCustomer"]),
                    TrangThai = "Chờ xử lý",

                };


                db.DonHang.Add(_order);

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
                return RedirectToAction("ShowCart");
            }

            var orderId = (int)Session["OrderId"];
            var order = db.DonHang.Find(orderId);
            if (order != null)
            {
                ViewBag.OrderId = order.IDdh;
                ViewBag.TotalAmount = order.Total_DH; // Gán giá trị mặc định nếu Total_DH là null
            }
            else
            {
                // Nếu không tìm thấy đơn hàng, chuyển hướng hoặc hiển thị thông báo lỗi
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng.";
                return RedirectToAction("ShowCart");
            }
            return View();
        }
    }
}