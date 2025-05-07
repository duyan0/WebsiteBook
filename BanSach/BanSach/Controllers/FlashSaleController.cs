using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class FlashSaleController : Controller
    {
        private db_Book db = new db_Book();

        public ActionResult Index()
        {
            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            var today = DateTime.Today;

            // Get the active flash sale
            var activeFlashSale = db.FlashSale
                .Where(fs => fs.NgayApDung == today
                          && fs.GioBatDau <= currentTime
                          && fs.GioKetThuc >= currentTime
                          && fs.TrangThai == "Hoạt động")
                .FirstOrDefault();

            // Get products in the active flash sale
            var flashSaleProductsQuery = db.SanPham
                .Include(sp => sp.FlashSale_SanPham)
                .Include(sp => sp.TheLoai)
                .Include(sp => sp.TacGia)
                .Include(sp => sp.NhaXuatBan)
                .Include(sp => sp.KhuyenMai);

            // Extract IDfs to a primitive type
            int? flashSaleId = activeFlashSale?.IDfs;

            // Apply filter based on flash sale ID
            var flashSaleProducts = flashSaleId.HasValue
                ? flashSaleProductsQuery
                    .Where(sp => sp.FlashSale_SanPham.Any(fss => fss.IDfs == flashSaleId.Value))
                    .ToList()
                : new List<SanPham>();

            ViewBag.ActiveFlashSale = activeFlashSale;
            return View(flashSaleProducts);
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
}