using BanSach.DesignPatterns.CommandPattern;
using BanSach.DesignPatterns.FactoryPattern;
using BanSach.DesignPatterns.RepositoryPattern;
using BanSach.DesignPatterns.ServicePattern;
using BanSach.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class AdminsController : Controller
    {
        private readonly IAdminFactory _adminFactory;
        private readonly IAdminRepository _adminRepository;
        private readonly IAdminService _adminService;
        private readonly db_Book _db;

        public AdminsController()
        {
            _adminFactory = new AdminFactory(); // Khởi tạo Factory trực tiếp
            _db = _adminFactory.CreateDbContext(); // Khởi tạo DbContext
            _adminRepository = new AdminRepository(_db); // Truyền DbContext vào Repository
            _adminService = new AdminService(_adminRepository); // Truyền Repository vào Service
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            // Kiểm tra xem id có giá trị không, nếu không trả về lỗi 400 (Bad Request)

            using (var db = _adminFactory.CreateDbContext())
            // Sử dụng factory để tạo DbContext, đặt trong using để tự động giải phóng tài nguyên
            {
                Admin admin = db.Admin.Find(id);
                // Tìm admin trong database dựa trên id
                if (admin == null) return HttpNotFound();
                // Nếu không tìm thấy admin, trả về lỗi 404 (Not Found)
                return View(admin);
                // Trả về view với dữ liệu admin để hiển thị form chỉnh sửa
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Admin admin)
        {
            if (ModelState.IsValid)
            // Kiểm tra dữ liệu đầu vào từ form có hợp lệ không
            {
                using (var db = _adminFactory.CreateDbContext())
                // Tạo DbContext bằng factory, dùng using để tự động giải phóng
                {
                    db.Entry(admin).State = EntityState.Modified;
                    // Đánh dấu đối tượng admin là đã thay đổi trong DbContext
                    db.SaveChanges();
                    // Lưu thay đổi vào database
                    return RedirectToAction("Index", "Admins");
                    // Chuyển hướng về danh sách admin sau khi lưu thành công
                }
            }
            return View(admin);
            // Nếu dữ liệu không hợp lệ, trả về view với dữ liệu hiện tại để người dùng sửa lại
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Admin admin = _adminRepository.GetAdminById(id.Value);
            if (admin == null)
            {
                return HttpNotFound();
            }

            return View(admin);
        }
        public ActionResult Index()
        {
            var _listAdmin = _db.Admin.ToList();
            return View(_listAdmin);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,HoTen,Email,DiaChi,SoDT,VaiTro,TKhoan,MKhau")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _adminService.CreateAdmin(admin);
                return RedirectToAction("Index");
            }
            return View(admin);
        }

        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) // Kiểm tra ID có hợp lệ không
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new db_Book()) // Khởi tạo DbContext
            {
                var command = new DeleteAdminCommand(id.Value, db); // Tạo command
                Admin admin = command.GetAdmin(); // Lấy thông tin admin
                if (admin == null) // Nếu không tìm thấy admin
                {
                    return HttpNotFound();
                }
                return View(admin); // Trả về view để hiển thị thông tin admin trước khi xóa
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new db_Book()) // Khởi tạo DbContext
            {
                var command = new DeleteAdminCommand(id, db); // Tạo command
                command.Execute(); // Thực thi lệnh xóa
                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách
            }
        }
        public ActionResult QuanLyDanhGia()
        {
            var danhGiaList = _db.DanhGiaSanPham
                .Include(d => d.SanPham) // Bao gồm navigation property nếu cần
                .Include(d => d.KhachHang)
                .ToList(); // Trả về List<DanhGiaSanPham>
            return View(danhGiaList);
        }

        // Phản hồi đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PhanHoiDanhGia(int idDanhGia, string phanHoi)
        {
            var danhGia = _db.DanhGiaSanPham.Find(idDanhGia);
            if (danhGia == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đánh giá.";
                return RedirectToAction("QuanLyDanhGia");
            }

            danhGia.PhanHoi = phanHoi;
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Phản hồi đã được cập nhật.";
            return RedirectToAction("QuanLyDanhGia");
        }

        // Xóa đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaDanhGia(int idDanhGia)
        {
            var danhGia = _db.DanhGiaSanPham.Find(idDanhGia);
            if (danhGia == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đánh giá.";
                return RedirectToAction("QuanLyDanhGia");
            }

            var idSanPham = danhGia.IDsp;
            _db.DanhGiaSanPham.Remove(danhGia);

            // Cập nhật điểm đánh giá trung bình
            var product = _db.SanPham.Find(idSanPham);
            var avgRating = _db.DanhGiaSanPham
                .Where(d => d.IDsp == idSanPham)
                .Average(d => (decimal?)d.DiemDanhGia) ?? 0;
            product.DiemDanhGiaTrungBinh = Math.Round(avgRating, 1);
            _db.Entry(product).State = System.Data.Entity.EntityState.Modified;

            _db.SaveChanges();

            TempData["SuccessMessage"] = "Đánh giá đã được xóa.";
            return RedirectToAction("QuanLyDanhGia");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose(); // Dispose DbContext được khởi tạo trong constructor
            }
            base.Dispose(disposing);
        }
       
    }
}

