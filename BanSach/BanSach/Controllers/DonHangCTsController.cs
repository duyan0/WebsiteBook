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
        private readonly db_Book db = new db_Book();
        public ActionResult Index(string searchString, int? page)
        {
            // Lấy chi tiết đơn hàng cùng với thông tin đơn hàng và sản phẩm liên quan
            var donHangCTs = db.DonHangCT.Include(d => d.DonHang).Include(d => d.SanPham).AsQueryable();

            // Áp dụng bộ lọc tìm kiếm nếu có searchString
            if (!String.IsNullOrEmpty(searchString))
            {
                // Tìm kiếm theo ID đơn hàng, tên sản phẩm, hoặc ID sản phẩm
                donHangCTs = donHangCTs.Where(d => d.DonHang.IDdh.ToString().Contains(searchString)
                                                 || d.SanPham.TenSP.Contains(searchString)
                                                 || d.SanPham.TrangThaiSach.Contains(searchString)
                                                 || d.SanPham.IDsp.ToString().Contains(searchString));
            }

            // Định nghĩa kích thước trang và số trang
            int pageSize = 10; // Số lượng item mỗi trang
            int pageNumber = (page ?? 1); // Nếu không có trang cụ thể, mặc định là trang 1

            // Sắp xếp chi tiết đơn hàng theo thứ tự ưu tiên của trạng thái đơn hàng
            donHangCTs = donHangCTs
                .OrderBy(d => d.DonHang.TrangThai == null || d.DonHang.TrangThai == "Chờ xử lý" ? 0 :
                              d.DonHang.TrangThai == "Đã xác nhận" ? 1 :
                              d.DonHang.TrangThai == "Đã nhận hàng" ? 2 : 3) // Sắp xếp theo thứ tự ưu tiên của trạng thái đơn hàng
                .ThenBy(d => d.DonHang.IDdh) // Sắp xếp tiếp theo theo ID đơn hàng
                .ThenBy(d => d.IDDonHang); // Sắp xếp tiếp theo theo ID chi tiết đơn hàng

            // Trả về danh sách đã phân trang và lọc đến view
            return View(donHangCTs.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Lấy danh sách chi tiết đơn hàng với ID đơn hàng được cung cấp
            var donHangCTs = db.DonHangCT
                .Include(ct => ct.SanPham)
                .Include(ct => ct.DonHang.KhachHang)
                .Where(ct => ct.IDDonHang == id)
                .ToList();

            // Kiểm tra nếu không có chi tiết đơn hàng nào được tìm thấy
            if (donHangCTs == null || !donHangCTs.Any())
            {
                return HttpNotFound();
            }

            // Lấy thông tin đơn hàng từ chi tiết đơn hàng đầu tiên
            var orderDetails = donHangCTs.FirstOrDefault().DonHang;
            ViewBag.OrderDetails = orderDetails;
            ViewBag.TrangThai = orderDetails.TrangThai; // Thêm dòng này

            // Trả về view với danh sách chi tiết đơn hàng
            return View(donHangCTs);
        }

        public ActionResult DetailsKH(int? id)
        {
            // Kiểm tra id null và trả về lỗi nếu không hợp lệ
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy danh sách chi tiết đơn hàng với ID đơn hàng được cung cấp
            var donHangCTs = db.DonHangCT
                .Include(ct => ct.SanPham)
                .Include(ct => ct.DonHang.KhachHang)
                .Where(ct => ct.DonHang.IDdh == id)
                .ToList();

            // Kiểm tra nếu không có chi tiết đơn hàng nào
            if (donHangCTs == null || !donHangCTs.Any())
            {
                return HttpNotFound();
            }

            // Lấy thông tin đơn hàng từ chi tiết đơn hàng đầu tiên
            var donHang = donHangCTs.FirstOrDefault()?.DonHang;
            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Gán thông tin đơn hàng vào ViewBag
            ViewBag.OrderDetails = donHang;

            // Tính tổng giá trị của đơn hàng
            decimal totalAmount = donHangCTs.Sum(ct => (ct.SoLuong * (decimal?)(ct.Gia ?? 0.0)) ?? 0m);
            ViewBag.TotalAmount = totalAmount;

            // Trả về view với danh sách chi tiết đơn hàng
            return View(donHangCTs);
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
            // Lọc và nhóm sản phẩm theo ID, nhưng chỉ lấy sản phẩm thuộc một loại cụ thể
            var query = db.DonHangCT
                            .Include(d => d.SanPham) // Lọc theo loại sản phẩm
                            .GroupBy(d => new
                            {
                                idPro = d.IDSanPham,
                                namePro = d.SanPham.TenSP,
                                imgPro = d.SanPham.HinhAnh,
                                price = d.SanPham.GiaBan
                            })
                            .OrderByDescending(gr => gr.Sum(d => d.SoLuong)) // Lấy tổng số lượng bán
                            .Take(15) // Lấy 10 sản phẩm bán chạy nhất
                            .Select(gr => new ViewModel
                            {
                                IdPro = gr.Key.idPro,
                                NamePro = gr.Key.namePro,
                                ImgPro = gr.Key.imgPro,
                                price = (decimal)gr.Key.price,
                                Sum_Quantity = (int)gr.Sum(d => d.SoLuong) // Tổng số lượng bán của sản phẩm
                            })
                            .ToList();

            return PartialView(query);
        }



        public ActionResult ExportOrderDetailsToExcel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var donHangCTs = db.DonHangCT.Include(d => d.DonHang)
                                          .Include(d => d.SanPham)
                                          .Where(d => d.IDDonHang == id)
                                          .ToList();

            if (donHangCTs == null || !donHangCTs.Any())
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

                // Thông tin đơn hàng (lấy từ chi tiết đơn hàng đầu tiên)
                var donHang = donHangCTs.First().DonHang;

                worksheet.Cell(currentRow, 1).Value = "Mã đơn hàng";
                worksheet.Cell(currentRow, 2).Value = donHang.IDdh;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "ID khách hàng";
                worksheet.Cell(currentRow, 2).Value = donHang.IDkh;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Tên khách hàng";
                worksheet.Cell(currentRow, 2).Value = donHang.KhachHang?.TenKH ?? "N/A"; // Kiểm tra null
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Số điện thoại";
                worksheet.Cell(currentRow, 2).Value = donHang.KhachHang?.SoDT ?? "N/A"; // Kiểm tra null
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Địa chỉ";
                worksheet.Cell(currentRow, 2).Value = donHang.DiaChi ?? "N/A"; // Kiểm tra null
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Ngày đặt hàng";
                worksheet.Cell(currentRow, 2).Value = donHang.NgayDatHang?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa đặt hàng";
                currentRow++;

                // Dòng trống
                currentRow++;

                // Tiêu đề bảng chi tiết đơn hàng
                worksheet.Cell(currentRow, 1).Value = "Chi tiết đơn hàng";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                currentRow++;

                // Tiêu đề cột
                worksheet.Cell(currentRow, 1).Value = "Sản phẩm";
                worksheet.Cell(currentRow, 2).Value = "Số lượng";
                worksheet.Cell(currentRow, 3).Value = "Giá";
                worksheet.Cell(currentRow, 4).Value = "Tổng tiền";
                worksheet.Row(currentRow).Style.Font.Bold = true; // Làm đậm dòng tiêu đề
                currentRow++;

                // Chi tiết đơn hàng (lặp qua tất cả các sản phẩm trong đơn hàng)
                foreach (var item in donHangCTs)
                {
                    worksheet.Cell(currentRow, 1).Value = item.SanPham.TenSP;
                    worksheet.Cell(currentRow, 2).Value = item.SoLuong;
                    worksheet.Cell(currentRow, 3).Value = $"{item.Gia:N0} VND";
                    worksheet.Cell(currentRow, 4).Value = $"{(item.SoLuong * item.Gia):N0} VND";
                    currentRow++;
                }

                // Trạng thái đơn hàng
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Trạng thái đơn hàng";
                worksheet.Cell(currentRow, 2).Value = donHang.TrangThai ?? "N/A";

                // Xuất file Excel
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ChiTietDonHang.xlsx");
                }
            }
        }
        public ActionResult RecentOrders()
        {
            var recentOrders = db.DonHangCT
                .Include(d => d.SanPham)
                .Include(d => d.DonHang)
                .Where(d => d.SanPham != null
                         && (d.DonHang.TrangThai == "Đã nhận hàng" || d.DonHang.TrangThai == "Đã xác nhận"))
                .GroupBy(d => d.IDSanPham) // Nhóm theo ID sản phẩm để tránh trùng lặp
                .Select(gr => gr.OrderByDescending(d => d.DonHang.NgayDatHang).FirstOrDefault()) // Lấy đơn hàng mới nhất trong mỗi nhóm
                .OrderByDescending(d => d.DonHang.NgayDatHang) // Sắp xếp theo ngày đặt hàng
                .Take(15) // Lấy 15 sản phẩm duy nhất
                .Select(d => new RecentOrderViewModel
                {
                    Id = d.SanPham.IDsp,
                    TenSanPham = d.SanPham.TenSP,
                    Gia = (int)d.Gia,
                    NgayDatHang = d.DonHang.NgayDatHang ?? DateTime.Now,
                    TrangThai = d.DonHang.TrangThai,
                    HinhAnh = d.SanPham.HinhAnh // Lấy đường dẫn của hình ảnh sản phẩm
                })
                .ToList();

            // Debug: In số lượng sản phẩm và chi tiết
            Console.WriteLine($"Số sản phẩm trong recentOrders: {recentOrders.Count}");
            foreach (var item in recentOrders)
            {
                Console.WriteLine($"ID: {item.Id}, Tên: {item.TenSanPham}, Ngày: {item.NgayDatHang}");
            }

            return PartialView("RecentOrders", recentOrders);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveAll()
        {
            try
            {
                // Lấy danh sách tất cả các sản phẩm có trạng thái "Hết hàng"
                var pdhethang = db.SanPham.Where(dh => dh.TrangThaiSach == "Hết hàng").ToList();

                foreach (var donHang in pdhethang)
                {
                    // Xóa các chi tiết đơn hàng có liên quan
                    var chiTietDonHangs = db.DonHangCT.Where(ct => ct.IDSanPham == donHang.IDsp).ToList();
                    foreach (var chiTiet in chiTietDonHangs)
                    {
                        db.DonHangCT.Remove(chiTiet);  // Xóa chi tiết đơn hàng
                    }

                    // Sau khi xóa chi tiết đơn hàng, xóa sản phẩm
                    db.SanPham.Remove(donHang);
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "Tất cả các sản phẩm hết hàng đã được xóa.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa các sản phẩm hết hàng: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDanhGia(int id, int idSanPham, string danhgia)
        {
            // Tìm chi tiết đơn hàng theo ID đơn hàng và sản phẩm
            var donHangCT = db.DonHangCT
                .FirstOrDefault(d => d.IDDonHang == id && d.IDSanPham == idSanPham);

            if (donHangCT == null)
            {
                return HttpNotFound();
            }

            // Cập nhật đánh giá cho chi tiết đơn hàng
            donHangCT.DanhGia = danhgia;
            db.SaveChanges();

            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Đánh giá của bạn đã được cập nhật.";

            // Quay lại trang chi tiết đơn hàng
            return RedirectToAction("DetailsKH", new { ID = id });
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
