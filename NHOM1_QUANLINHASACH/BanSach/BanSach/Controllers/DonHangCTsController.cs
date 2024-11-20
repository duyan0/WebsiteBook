using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using BanSach.Models;
using ClosedXML.Excel;
using PagedList;


namespace BanSach.Controllers
{
    public class DonHangCTsController : Controller
    {
        private dbSach db = new dbSach();
        public ActionResult Index(string searchString, int? page)
        {
            // Get order details along with related order and product information
            var donHangCTs = db.DonHangCT.Include(d => d.DonHang).Include(d => d.SanPham).AsQueryable();

            // Apply search filter if searchString is provided
            if (!String.IsNullOrEmpty(searchString))
            {
                // Search by Order ID, Product Name, or Product ID
                donHangCTs = donHangCTs.Where(d => d.DonHang.IDdh.ToString().Contains(searchString)
                                                 || d.SanPham.TenSP.Contains(searchString)
                                                 || d.SanPham.IDsp.ToString().Contains(searchString));
            }

            // Define page size and number
            int pageSize = 10; // Number of items per page
            int pageNumber = (page ?? 1); // Default to page 1 if no page is specified

            // Return the paginated and filtered list to the view
            return View(donHangCTs.OrderBy(d => d.IDDonHang).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHangCT donHangCT = db.DonHangCT.Find(id);
            if (donHangCT == null)
            {
                return HttpNotFound();
            }
            return View(donHangCT);
        }
        public ActionResult DetailsKH(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHangCT donHangCT = db.DonHangCT.Find(id);
            if (donHangCT == null)
            {
                return HttpNotFound();
            }
            return View(donHangCT);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHangCT donHangCT = db.DonHangCT.Find(id);
            if (donHangCT == null)
            {
                return HttpNotFound();
            }
            return View(donHangCT);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DonHangCT donHangCT = db.DonHangCT.Find(id);
            db.DonHangCT.Remove(donHangCT);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [ChildActionOnly]
        public PartialViewResult TopBanChay()
        {
            List<DonHangCT> orderD = db.DonHangCT.ToList();
            List<SanPham> prolist = db.SanPham.ToList();
            var query = from od in orderD join p in prolist on od.IDSanPham equals p.IDsp into tbl
                        group od by new { idPro = od.IDSanPham,
                            namePro = od.SanPham.TenSP,
                            imgPro = od.SanPham.HinhAnh,
                            price = od.SanPham.GiaBan 
                        } into gr
                        orderby gr.Sum(s => s.SoLuong) descending
                        select new ViewModel
                        {
                            IdPro = gr.Key.idPro,
                            NamePro = gr.Key.namePro,
                            ImgPro = gr.Key.imgPro,
                            price = (decimal)gr.Key.price,
                            Sum_Quantity = gr.Sum(s => s.SoLuong)
                        };


            return PartialView(query.Take(10).ToList());
        }
        public ActionResult ExportOrderDetailsToExcel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var donHangCT = db.DonHangCT.Include(d => d.DonHang)
                                         .Include(d => d.SanPham)
                                         .FirstOrDefault(d => d.IDDonHang == id);

            if (donHangCT == null)
            {
                return HttpNotFound();
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ChiTietDonHang");
                var currentRow = 1;

                // Tiêu đề bảng thông tin đơn hàng
                worksheet.Cell(currentRow, 1).Value = "Thông tin đơn hàng";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                currentRow++;

                // Thông tin đơn hàng
                worksheet.Cell(currentRow, 1).Value = "Mã đơn hàng";
                worksheet.Cell(currentRow, 2).Value = donHangCT.IDDonHang;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "ID khách hàng";
                worksheet.Cell(currentRow, 2).Value = donHangCT.DonHang.IDkh;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Tên khách hàng";
                worksheet.Cell(currentRow, 2).Value = donHangCT.DonHang.KhachHang.TenKH;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Số điện thoại";
                worksheet.Cell(currentRow, 2).Value = donHangCT.DonHang.KhachHang.SoDT;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Địa chỉ";
                worksheet.Cell(currentRow, 2).Value = donHangCT.DonHang.DiaChi;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Ngày đặt hàng";
                worksheet.Cell(currentRow, 2).Value = donHangCT.DonHang.NgayDatHang?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa đặt hàng";
                currentRow++;

                // Dòng trống
                currentRow++;

                // Tiêu đề bảng chi tiết đơn hàng
                worksheet.Cell(currentRow, 1).Value = "Chi tiết đơn hàng";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                currentRow++;

                // Chi tiết đơn hàng
                worksheet.Cell(currentRow, 1).Value = "Sản phẩm";
                worksheet.Cell(currentRow, 2).Value = donHangCT.SanPham.TenSP;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Số lượng";
                worksheet.Cell(currentRow, 2).Value = donHangCT.SoLuong;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Giá";
                worksheet.Cell(currentRow, 2).Value = $"{donHangCT.Gia:N0} VND";
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Tổng tiền";
                worksheet.Cell(currentRow, 2).Value = $"{donHangCT.TongTien:N0} VND";
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Trạng thái";
                worksheet.Cell(currentRow, 2).Value = donHangCT.DonHang.TrangThai;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ChiTietDonHang.xlsx");
                }
            }
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
