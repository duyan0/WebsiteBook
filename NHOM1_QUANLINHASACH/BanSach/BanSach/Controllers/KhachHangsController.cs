using BanSach.Models;
using ClosedXML.Excel;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace BanSach.Controllers
{

    public class KhachHangsController : Controller
    {
        private dbSach db = new dbSach();
        public ActionResult LoginCus()
        {
            return View();
        }

        // GET: KhachHangs
        [HttpGet]
        public ActionResult Index(int? page, string searchString)
        {
            int pageSize = 10; // Số bản ghi trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1
            ViewBag.CurrentFilter = searchString;
            var khachhang = db.KhachHang.AsQueryable();

            // Nếu có từ khóa tìm kiếm thì lọc danh sách Admin
            if (!string.IsNullOrEmpty(searchString))
            {
                khachhang = khachhang.Where(a => a.TenKH.Contains(searchString) ||
                                           a.SoDT.Contains(searchString) ||
                                           a.Email.Contains(searchString) ||
                                           a.TKhoan.Contains(searchString));
            }
            var khachHangList = khachhang.OrderBy(kh => kh.IDkh).ToPagedList(pageNumber, pageSize);
            return View(khachHangList);
        }

        // GET: KhachHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }
        public ActionResult DetailsAD(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // GET: KhachHangs/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDkh,TenKH,SoDT,Email,TKhoan,MKhau")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại chưa
                var existingEmail = db.KhachHang.FirstOrDefault(kh => kh.Email == khachHang.Email);
                if (existingEmail != null)
                {
                    // Nếu email đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("Email", "Email này đã tồn tại. Vui lòng sử dụng email khác.");
                    return View(khachHang);
                }

                // Kiểm tra xem số điện thoại đã tồn tại chưa
                var existingPhoneNumber = db.KhachHang.FirstOrDefault(kh => kh.SoDT == khachHang.SoDT);
                if (existingPhoneNumber != null)
                {
                    // Nếu số điện thoại đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("SoDT", "Số điện thoại này đã tồn tại. Vui lòng sử dụng số điện thoại khác.");
                    return View(khachHang);
                }

                // Nếu cả email và số điện thoại không tồn tại, thêm đối tượng mới vào cơ sở dữ liệu
                db.KhachHang.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(khachHang);
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang kh = db.KhachHang.Find(id);
            if (kh == null)
            {
                return HttpNotFound();
            }
            return View(kh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDkh,TenKH,SoDT,Email,TKhoan,MKhau")] KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kh).State = EntityState.Modified;

                // Nếu bạn cần ValidateOnSaveEnabled = false
                db.Configuration.ValidateOnSaveEnabled = false;

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("UpdateSuccesss", "KhachHangs");
                }
                catch (Exception ex)
                {
                    // Log lỗi để kiểm tra thêm chi tiết nếu cần
                    Console.WriteLine(ex.Message);
                }
            }

            return View(kh);
        }
        public ActionResult EditAD(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang kh = db.KhachHang.Find(id);
            if (kh == null)
            {
                return HttpNotFound();
            }
            return View(kh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAD([Bind(Include = "IDkh,TenKH,SoDT,Email,TKhoan,MKhau")] KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kh).State = EntityState.Modified;

                // Nếu bạn cần ValidateOnSaveEnabled = false
                db.Configuration.ValidateOnSaveEnabled = false;

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index", "KhachHangs");
                }
                catch (Exception ex)
                {
                    // Log lỗi để kiểm tra thêm chi tiết nếu cần
                    Console.WriteLine(ex.Message);
                }
            }

            return View(kh);
        }
        [HttpGet]
        public ActionResult UpdateSuccesss()
        {
            return View();
        }
        // GET: KhachHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KhachHang khachHang = db.KhachHang.Find(id);
            db.KhachHang.Remove(khachHang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult LichSuDonHang(int? page)
        {
            int? currentCustomerId = Session["IDkh"] as int?;

            if (!currentCustomerId.HasValue)
            {
                return RedirectToAction("LoginAccountCus", "LoginUser");
            }

            // Truy vấn đơn hàng và sắp xếp theo ngày đặt hàng giảm dần
            var orders = db.DonHang
                .Where(dh => dh.IDkh == currentCustomerId.Value)
                .OrderByDescending(dh => dh.NgayDatHang) // Sắp xếp theo ngày đặt hàng giảm dần
                .Include(dh => dh.DonHangCT)
                .ToList();

            int pageSize = 10; // Số đơn hàng mỗi trang
            int pageNumber = (page ?? 1);

            // Chuyển đổi từ List sang IPagedList
            var pagedOrders = orders.ToPagedList(pageNumber, pageSize);

            return View(pagedOrders);
        }



        [HttpPost]
        public async Task<ActionResult> HuyDonHang(int orderId)
        {
            int? currentCustomerId = Session["IDkh"] as int?;

            if (!currentCustomerId.HasValue)
            {
                return RedirectToAction("LoginAccountCus", "LoginUser");
            }

            // Tìm đơn hàng theo ID và kiểm tra quyền sở hữu
            var order = await db.DonHang.FirstOrDefaultAsync(dh => dh.IDdh == orderId && dh.IDkh == currentCustomerId.Value);

            if (order == null)
            {
                return HttpNotFound(); // Hoặc hiển thị thông báo lỗi phù hợp
            }

            // Cập nhật trạng thái đơn hàng thành "Canceled"
            order.TrangThai = "Đã hủy";

            // Lưu thay đổi vào cơ sở dữ liệu
            await db.SaveChangesAsync();

            // Quay lại lịch sử đơn hàng
            return RedirectToAction("LichSuDonHang");
        }
        public async Task<ActionResult> ExportToExcel()
        {
            int? currentCustomerId = Session["IDkh"] as int?;

            if (!currentCustomerId.HasValue)
            {
                return RedirectToAction("LoginAccountCus", "LoginUser");
            }

            var orders = await db.DonHang
                .Where(dh => dh.IDkh == currentCustomerId.Value)
                .Include(dh => dh.DonHangCT)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("LichSuDonHang");
                var currentRow = 1;

                // Tiêu đề cột
                worksheet.Cell(currentRow, 1).Value = "Mã đơn hàng";
                worksheet.Cell(currentRow, 2).Value = "Ngày đặt hàng";
                worksheet.Cell(currentRow, 3).Value = "Ngày nhận hàng";
                worksheet.Cell(currentRow, 4).Value = "Tổng số tiền";
                worksheet.Cell(currentRow, 5).Value = "Trạng thái";

                // Nội dung đơn hàng
                foreach (var order in orders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = order.IDdh;
                    worksheet.Cell(currentRow, 2).Value = order.NgayDatHang?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa đặt hàng";
                    worksheet.Cell(currentRow, 3).Value = order.TrangThai == "Đã nhận hàng" && order.NgayNhanHang.HasValue
                        ? order.NgayNhanHang.Value.ToString("dd/MM/yyyy HH:mm:ss")
                        : (order.TrangThai == "Đã xác nhận" && order.NgayDatHang.HasValue
                            ? "Dự kiến: " + order.NgayDatHang.Value.AddDays(2).ToString("dd/MM/yyyy")
                            : "Chưa có thông tin");
                    worksheet.Cell(currentRow, 4).Value = order.GetTongSoTien().ToString("C0", new System.Globalization.CultureInfo("vi-VN"));
                    worksheet.Cell(currentRow, 5).Value = order.TrangThai;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LichSuDonHang.xlsx");
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