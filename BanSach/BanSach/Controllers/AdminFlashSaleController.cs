using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    
    public class AdminFlashSaleController : Controller
    {
        private db_Book db = new db_Book();

        public ActionResult Index()
        {
            var flashSales = db.FlashSale.ToList();
            return View(flashSales);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FlashSale flashSale)
        {
            if (ModelState.IsValid)
            {
                flashSale.TrangThai = "Hoạt động";
                db.FlashSale.Add(flashSale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(flashSale);
        }

        public ActionResult ManageProducts(int id)
        {
            var flashSale = db.FlashSale.Find(id);
            if (flashSale == null) return HttpNotFound();

            var model = new FlashSaleProductViewModel
            {
                FlashSaleId = id,
                FlashSaleName = flashSale.TenFlashSale,
                AvailableProducts = db.SanPham.ToList(),
                SelectedProductIds = db.FlashSale_SanPham
                    .Where(fss => fss.IDfs == id)
                    .Select(fss => fss.IDsp)
                    .ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProducts(FlashSaleProductViewModel model)
        {
            var flashSale = db.FlashSale.Find(model.FlashSaleId);
            if (flashSale == null) return HttpNotFound();

            // Remove existing product associations
            var existing = db.FlashSale_SanPham.Where(fss => fss.IDfs == model.FlashSaleId);
            db.FlashSale_SanPham.RemoveRange(existing);

            // Add new product associations
            if (model.SelectedProductIds != null)
            {
                foreach (var productId in model.SelectedProductIds)
                {
                    var product = db.SanPham.Find(productId);
                    if (product != null)
                    {
                        if (product.KhuyenMai != null && product.KhuyenMai.NgayKetThuc >= DateTime.Today)
                        {
                            ModelState.AddModelError("", $"Sản phẩm {product.TenSP} đang có khuyến mãi thông thường. Vui lòng kiểm tra.");
                            model.AvailableProducts = db.SanPham.ToList();
                            return View(model);
                        }
                        db.FlashSale_SanPham.Add(new FlashSale_SanPham { IDfs = model.FlashSaleId, IDsp = productId });
                    }
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class FlashSaleProductViewModel
    {
        public int FlashSaleId { get; set; }
        public string FlashSaleName { get; set; }
        public List<SanPham> AvailableProducts { get; set; }
        public List<int> SelectedProductIds { get; set; }
    }
}