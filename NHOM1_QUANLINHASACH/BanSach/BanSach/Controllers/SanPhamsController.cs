using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BanSach.Models;
using PagedList;

namespace BanSach.Controllers
{
    public class SanPhamsController : Controller
    {
        private dbSach db = new dbSach();

        // GET: SanPhams
        public ActionResult Index(string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;

            // Lấy danh sách tất cả sản phẩm từ cơ sở dữ liệu
            var sanPhams = db.SanPham.Include(s => s.DanhMuc).Include(s => s.TacGia).Include(s => s.NhaXuatBan);

            // Nếu có từ khóa tìm kiếm, lọc sản phẩm theo tên
            if (!String.IsNullOrEmpty(searchString))
            {
                sanPhams = sanPhams.Where(s => s.TenSP.Contains(searchString)
                                             || s.TacGia.TenTacGia.Contains(searchString)
                                             || s.DanhMuc.TheLoai.Contains(searchString)
                                             || s.NhaXuatBan.Tennxb.Contains(searchString));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            // Trả về kết quả đã được phân trang
            return View(sanPhams.OrderBy(s => s.IDsp).ToPagedList(pageNumber, pageSize));
        }


        public ActionResult TrangChu()
        {
            return View();
        }
        public ActionResult ProductList(int? category, int? page, string SearchString )
        {
            var products = db.SanPham.Include(p => p.DanhMuc);
            
            int pageSize = 12;           
            int pageNumber = (page ?? 1);
            if (page == null) page = 1;

            
            // Tìm kiếm chuỗi truy vấn theo category
            if (category == null)
            {
                products = db.SanPham.OrderByDescending(x => x.TenSP);
            }
            else
            {
                products = db.SanPham.OrderByDescending(x => x.TheLoai).Where(x => x.TheLoai == category);
            }

            // Tìm kiếm chuỗi truy vấn theo NamePro (SearchString)
            if (!String.IsNullOrEmpty(SearchString))
            {
                products = db.SanPham.OrderByDescending(x => x.TheLoai).Where(s => s.TenSP.Contains(SearchString));
            }
          
            // Trả về các product được phân trang theo kích thước và số trang.
            return View(products.ToPagedList(pageNumber, pageSize));

        }
        // Xem SP
        public ActionResult TrangSP(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPham.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }
        // GET: SanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPham.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }
        public ActionResult Create()
        {
            ViewBag.TheLoai = new SelectList(db.DanhMuc, "ID", "TheLoai");
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTacGia");
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb");

            // Điều chỉnh ViewBag.KM để hiển thị MucGiamGia và TenKm kết hợp
            ViewBag.KM = new SelectList(db.KhuyenMai.ToList().Select(km => new
            {
                IDKM = km.IDkm,
                MucGiamGiaTenKm = km.MucGiamGia + "% - " + km.TenKhuyenMai
            }), "IDkm", "MucGiamGiaTenKm");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDsp,TenSP,MoTa,TheLoai,GiaBan,HinhAnh,IDtg,IDnxb,IDkm,SoLuong,TrangThaiSach")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.SanPham.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TheLoai = new SelectList(db.DanhMuc, "ID", "TheLoai", sanPham.TheLoai);
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTacGia", sanPham.TacGia);
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb", sanPham.NhaXuatBan);
            ViewBag.KM = new SelectList(db.KhuyenMai.ToList().Select(km => new
            {
                IDKM = km.IDkm,
                MucGiamGiaTenKm = km.MucGiamGia + "% - " + km.TenKhuyenMai
            }), "IDkm", "MucGiamGiaTenKm", sanPham.KhuyenMai);


            return View(sanPham);
        }



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham product = db.SanPham.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Cập nhật ViewBag.TheLoai, ViewBag.TacGia, ViewBag.NXB, và ViewBag.KM
            ViewBag.TheLoai = new SelectList(db.DanhMuc, "ID", "TheLoai", product.TheLoai);
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTacGia", product.TacGia);
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb", product.NhaXuatBan);
            ViewBag.KM = new SelectList(db.KhuyenMai.ToList().Select(km => new
            {
                IDkm = km.IDkm,
                MucGiamGiaTenKm = km.MucGiamGia + "% - " + km.TenKhuyenMai
            }), "IDkm", "MucGiamGiaTenKm", product.KhuyenMai);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDsp,TenSP,MoTa,TheLoai,GiaBan,HinhAnh,IDtg,IDnxb,IDkm,SoLuong,TrangThaiSach")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Cập nhật ViewBag.TheLoai, ViewBag.TacGia, ViewBag.NXB, và ViewBag.KM khi model state không hợp lệ
            ViewBag.TheLoai = new SelectList(db.DanhMuc, "ID", "TheLoai", sanPham.TheLoai);
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTacGia", sanPham.TacGia);
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb", sanPham.NhaXuatBan);
            ViewBag.KM = new SelectList(db.KhuyenMai.ToList().Select(km => new
            {
                IDkm = km.IDkm,
                MucGiamGiaTenKm = km.MucGiamGia + "% - " + km.TenKhuyenMai
            }), "IDkm", "MucGiamGiaTenKm", sanPham.KhuyenMai);

            return View(sanPham);
        }


        // GET: SanPhams/Delete/5
        public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SanPham sanPham = db.SanPham.Find(id);
                if (sanPham == null)
                {
                    return HttpNotFound();
                }
                return View(sanPham);
            }

        // POST: SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanPham = db.SanPham.Find(id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra nếu sản phẩm này đã tồn tại trong chi tiết đơn hàng nào đó
            bool isReferencedInOrder = db.DonHangCT.Any(dhct => dhct.IDSanPham == id);

            if (isReferencedInOrder)
            {
                // Nếu sản phẩm có liên kết với đơn hàng, không xóa và trả về thông báo lỗi
                ModelState.AddModelError("", "Không thể xóa sản phẩm này vì nó đã có trong đơn hàng.");
                return View("Delete", sanPham); // Trả lại View Delete để hiển thị thông báo
            }

            // Nếu không có liên kết nào, thực hiện xóa
            db.SanPham.Remove(sanPham);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        // Thêm phương thức lọc sản phẩm theo giá
        public ActionResult FilterByPrice(List<string> priceRange, int? page)
        {
            // Lấy danh sách tất cả sản phẩm
            var sanPhams = db.SanPham.AsQueryable();

            // Nếu có phạm vi giá được chọn, áp dụng điều kiện lọc
            if (priceRange != null && priceRange.Any())
            {
                var filteredProducts = new List<SanPham>();

                foreach (var range in priceRange)
                {
                    // Tách giá trị thấp và cao từ priceRange (dạng "0-150000" hoặc "700000-up")
                    var values = range.Split('-');

                    if (values.Length == 2 && decimal.TryParse(values[0], out var minPrice))
                    {
                        if (values[1] == "up")
                        {
                            // Nếu giá trị là "700000-up", lọc những sản phẩm có giá từ 700000 trở lên
                            filteredProducts.AddRange(sanPhams.Where(sp => sp.GiaBan >= minPrice).ToList());
                        }
                        else if (decimal.TryParse(values[1], out var maxPrice))
                        {
                            // Nếu giá trị là dạng số (ví dụ "0-150000"), lọc theo khoảng giá
                            filteredProducts.AddRange(sanPhams.Where(sp => sp.GiaBan >= minPrice && sp.GiaBan <= maxPrice).ToList());
                        }
                    }
                }

                // Xóa các bản sao trùng lặp (nếu có)
                sanPhams = filteredProducts.Distinct().AsQueryable();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            // Trả về kết quả đã được phân trang
            return View("ProductList", sanPhams.OrderBy(s => s.IDsp).ToPagedList(pageNumber, pageSize));
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
