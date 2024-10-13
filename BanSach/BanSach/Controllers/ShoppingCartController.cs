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
        SachEntities db = new SachEntities();
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
            // lấy sản phẩm theo id
            var _pro = db.SanPham.SingleOrDefault(s => s.IDsp == id);
            if (_pro != null)
            {
                GetCart().Add_Product_Cart(_pro);
            }
            return RedirectToAction("TrangSP", "SanPhams", new {id});
        }
        //Mua ngay chuyển tới giỏ hàng
        public ActionResult MuaNgay(int id)
        {
            // lấy sản phẩm theo id
            var _pro = db.SanPham.SingleOrDefault(s => s.IDsp == id);
            if (_pro != null)
            {
                GetCart().Add_Product_Cart(_pro);
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
                DonHang _order = new DonHang();
                _order.NgayDatHang = DateTime.Now;
                _order.DiaChi = form["AddressDeliverry"];
                _order.IDkh = int.Parse(form["CodeCustomer"]);
                
                db.DonHang.Add(_order);
                foreach (var item in cart.Items)
                {
                    // lưu dòng sản phẩm vào chi tiết hóa đơn
                    DonHangCT _order_detail = new DonHangCT();
                    _order_detail.IDDonHang = _order.IDdh;
                    _order_detail.IDSanPham = item._product.IDsp;
                    _order_detail.Gia = (double)item._product.GiaBan;
                    _order_detail.SoLuong = item._quantity;
                    db.DonHangCT.Add(_order_detail);
                    //xử lý cập nhật lại số lượng tồn trong bảng Product
                    foreach (var p in db.SanPham.Where(s => s.IDsp == _order_detail.IDSanPham)) //lấy ID Product đang có trong giỏ hàng
                    {
                        var update_quan_pro = p.SoLuong - item._quantity; //số lượng tồn mới = số lượng tồn - số lượng đã mua
                        p.SoLuong = update_quan_pro; //thực hiện cập nhật lại số lượng tồn cho cột Quantity của bảng Product
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