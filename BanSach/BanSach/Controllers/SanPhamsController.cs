using BanSach.Models;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class SanPhamsController : Controller
    {
        private readonly db_Book db = new db_Book();

        // GET: SanPhams
        public ActionResult Index(string searchString, string sortOrder, int? page)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;

            // Các tùy chọn sắp xếp
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParam = sortOrder == "Price" ? "price_desc" : "Price";
            ViewBag.AuthorSortParam = sortOrder == "Author" ? "author_desc" : "Author";
            ViewBag.StatusSortParam = sortOrder == "Status" ? "status_desc" : "Status";

            var sanPhams = db.SanPham.Include(s => s.TheLoai).Include(s => s.TacGia).Include(s => s.NhaXuatBan);

            // Tìm kiếm theo từ khóa
            if (!String.IsNullOrEmpty(searchString))
            {
                sanPhams = sanPhams.Where(s => s.TenSP.Contains(searchString)
                                             || s.TacGia.TenTG.Contains(searchString)
                                             || s.TheLoai.TenTheLoai.Contains(searchString)
                                             || s.TrangThaiSach.Contains(searchString)
                                             || s.NhaXuatBan.Tennxb.Contains(searchString));
            }

            // Sắp xếp theo các tiêu chí
            switch (sortOrder)
            {
                case "name_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.TenSP);
                    break;
                case "Price":
                    sanPhams = sanPhams.OrderBy(s => s.GiaBan);
                    break;
                case "price_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.GiaBan);
                    break;
                case "Author":
                    sanPhams = sanPhams.OrderBy(s => s.SoLuong);
                    break;
                case "author_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.SoLuong);
                    break;
                case "Status":
                    sanPhams = sanPhams.OrderBy(s => s.TrangThaiSach);
                    break;
                case "status_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.TrangThaiSach);
                    break;
                default:
                    sanPhams = sanPhams.OrderBy(s => s.IDsp);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            // Trả về danh sách sản phẩm đã phân trang dưới dạng PartialView
            return PartialView("Index", sanPhams.ToPagedList(pageNumber, pageSize));
        }



        public ActionResult TrangChu()
        {
            var danhMucList = db.DanhMuc.ToList();
            return View(danhMucList);


        }

        public ActionResult ProductList(int? category, int? page, string SearchString, string sortOrder)
        {
            SetAvailablePublishers();

            // Khởi tạo truy vấn
            var products = db.SanPham
                            .Include(p => p.TheLoai)
                            .Include(p => p.KhuyenMai) // Bao gồm bảng KhuyenMai để truy cập MucGiamGia
                            .AsQueryable();

            int pageSize = 18;
            int pageNumber = (page ?? 1);

            // Lọc theo category
            if (category.HasValue)
            {
                products = products.Where(x => x.IDtl == category.Value);
            }

            // Lọc theo chuỗi tìm kiếm (SearchString)
            if (!string.IsNullOrEmpty(SearchString))
            {
                products = products.Where(s => s.TenSP.Contains(SearchString));
            }

            // Sắp xếp theo giá (bao gồm giảm giá) hoặc tên sản phẩm
            switch (sortOrder)
            {
                case "price_desc":
                    // Sắp xếp theo giá giảm dần, ưu tiên giá sau giảm nếu có khuyến mãi
                    products = products.OrderByDescending(x =>
                        x.KhuyenMai != null && x.KhuyenMai.MucGiamGia > 0
                        ? x.GiaBan * (1 - (decimal)x.KhuyenMai.MucGiamGia / 100)
                        : x.GiaBan);
                    break;
                case "price_asc":
                    // Sắp xếp theo giá tăng dần, ưu tiên giá sau giảm nếu có khuyến mãi
                    products = products.OrderBy(x =>
                        x.KhuyenMai != null && x.KhuyenMai.MucGiamGia > 0
                        ? x.GiaBan * (1 - (decimal)x.KhuyenMai.MucGiamGia / 100)
                        : x.GiaBan);
                    break;
                default:
                    // Mặc định theo tên sản phẩm tăng dần
                    products = products.OrderBy(x => x.TenSP);
                    break;
            }

            // Trả về kết quả phân trang
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

            // Mã hóa URL liên kết sản phẩm
            string encodedUrl = HttpUtility.UrlEncode(Url.Action("TrangSP", "SanPhams", new { id = sanPham.IDsp }, Request.Url.Scheme));

            ViewBag.EncodedUrl = encodedUrl;

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
            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai");
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTG");
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb");

            ViewBag.KM = new SelectList(
                db.KhuyenMai.ToList().Select(km => new
                {
                    IDKM = km.IDkm,
                    MucGiamGiaTenKm = $"{km.MucGiamGia}% - {km.TenKhuyenMai}"
                }),
                "IDKM",
                "MucGiamGiaTenKm"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.SanPham.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai", sanPham.TheLoai);
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTG", sanPham.TacGia);
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb", sanPham.NhaXuatBan);
            ViewBag.KM = new SelectList(db.KhuyenMai.ToList().Select(km => new
            {
                IDKM = km.IDkm,
                MucGiamGiaTenKm = $"{km.MucGiamGia}% - {km.TenKhuyenMai}"
            }),
                "IDKM",
                "MucGiamGiaTenKm",
                sanPham.IDkm
            );

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

            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai", product.TheLoai);
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTG", product.TacGia);
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb", product.NhaXuatBan);

            ViewBag.KM = new SelectList(
                db.KhuyenMai.ToList().Select(km => new
                {
                    IDKM = km.IDkm,
                    MucGiamGiaTenKm = $"{km.MucGiamGia}% - {km.TenKhuyenMai}"
                }),
                "IDKM",
                "MucGiamGiaTenKm",
                product.IDkm
            );

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai", sanPham.TheLoai);
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTG", sanPham.TacGia);
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb", sanPham.NhaXuatBan);
            ViewBag.KM = new SelectList(db.KhuyenMai.ToList().Select(km => new
            {
                IDKM = km.IDkm,
                MucGiamGiaTenKm = $"{km.MucGiamGia}% - {km.TenKhuyenMai}"
            }),
                "IDKM",
                "MucGiamGiaTenKm",
                sanPham.IDkm
            );

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
        public ActionResult FilterProducts(int? page, string[] publisherFilters, string sortOrder)
        {
            var sanPhams = db.SanPham.AsQueryable();

            // Kiểm tra và lấy danh sách nhà xuất bản từ cơ sở dữ liệu
            System.Diagnostics.Debug.WriteLine("Kiểm tra DB: " + db.NhaXuatBan.Count());
            var publisherList = db.NhaXuatBan.Select(x => x.Tennxb).ToList();
            System.Diagnostics.Debug.WriteLine("Số nhà xuất bản từ DB: " + publisherList.Count);
            System.Diagnostics.Debug.WriteLine("Danh sách nhà xuất bản: " + string.Join(", ", publisherList));

            // Thêm dữ liệu mẫu nếu không có dữ liệu
            if (!publisherList.Any())
            {
                System.Diagnostics.Debug.WriteLine("Cơ sở dữ liệu rỗng, thêm dữ liệu mẫu.");
                db.NhaXuatBan.Add(new NhaXuatBan { Tennxb = "Nhà xuất bản Trẻ" });
                db.NhaXuatBan.Add(new NhaXuatBan { Tennxb = "Nhà xuất bản duy ấn" });
                db.SaveChanges();
                publisherList = db.NhaXuatBan.Select(x => x.Tennxb).ToList(); // Cập nhật danh sách
            }

            // Tạo slug từ danh sách đã lấy
            var publishers = publisherList.Select(p => new { Original = p, Slug = SlugHelper.GenerateSlug(p) }).ToList();
            System.Diagnostics.Debug.WriteLine("Số nhà xuất bản sau khi tạo slug: " + publishers.Count);

            // Kiểm tra và thêm giá trị mặc định nếu rỗng
            if (!publishers.Any())
            {
                System.Diagnostics.Debug.WriteLine("Không có nhà xuất bản, thêm mặc định.");
                publishers.Add(new { Original = "Không có Nhà Xuất Bản khả dụng", Slug = "khong-co-nha-xuat-ban-kha-dung" });
            }

            // Gán danh sách vào ViewBag
            ViewBag.AvailablePublishers = publishers;
            System.Diagnostics.Debug.WriteLine("ViewBag.AvailablePublishers đã gán: " + publishers.Count);

            // Lọc theo nhà xuất bản (ánh xạ ngược từ slug về tên gốc)
            if (publisherFilters != null && publisherFilters.Any())
            {
                var slugToOriginal = publishers.ToDictionary(p => p.Slug, p => p.Original);
                var originalNames = publisherFilters.Select(slug => slugToOriginal.TryGetValue(slug, out var original) ? original : null)
                                                    .Where(name => name != null)
                                                    .ToList();
                if (originalNames.Any())
                {
                    sanPhams = sanPhams.Where(p => originalNames.Contains(p.NhaXuatBan.Tennxb));
                }
            }

            // Lọc theo phạm vi giá
            var priceRangeValues = Request.QueryString.GetValues("priceRange");
            if (priceRangeValues != null && priceRangeValues.Any())
            {
                var priceConditions = new List<Expression<Func<SanPham, bool>>>();
                foreach (var range in priceRangeValues)
                {
                    var values = range.Split('-');
                    if (values.Length == 2 && decimal.TryParse(values[0], out var minPrice))
                    {
                        if (values[1] == "up")
                        {
                            priceConditions.Add(sp => sp.GiaBan >= minPrice);
                        }
                        else if (decimal.TryParse(values[1], out var maxPrice))
                        {
                            priceConditions.Add(sp => sp.GiaBan >= minPrice && sp.GiaBan <= maxPrice);
                        }
                    }
                }
                if (priceConditions.Any())
                {
                    sanPhams = priceConditions.Aggregate(sanPhams, (current, condition) => current.Where(condition));
                }
            }

            // Sắp xếp theo giá
            switch (sortOrder)
            {
                case "price_asc":
                    sanPhams = sanPhams.OrderBy(s =>
                        s.KhuyenMai != null && s.KhuyenMai.MucGiamGia > 0
                        ? s.GiaBan * (1 - (decimal)s.KhuyenMai.MucGiamGia / 100)
                        : s.GiaBan);
                    break;
                case "price_desc":
                    sanPhams = sanPhams.OrderByDescending(s =>
                        s.KhuyenMai != null && s.KhuyenMai.MucGiamGia > 0
                        ? s.GiaBan * (1 - (decimal)s.KhuyenMai.MucGiamGia / 100)
                        : s.GiaBan);
                    break;
                default:
                    sanPhams = sanPhams.OrderBy(s => s.IDsp);
                    break;
            }

            // Phân trang
            int pageSize = 16;
            int pageNumber = (page ?? 1);
            return View("ProductList", sanPhams.ToPagedList(pageNumber, pageSize));
        }
        private void SetAvailablePublishers()
        {
            System.Diagnostics.Debug.WriteLine("SetAvailablePublishers - Kiểm tra DB: " + db.NhaXuatBan.Count());
            var publishers = db.NhaXuatBan.Select(nxb => nxb.Tennxb).ToList();
            System.Diagnostics.Debug.WriteLine("Số nhà xuất bản trong SetAvailablePublishers: " + publishers.Count);
            ViewBag.AvailablePublishers = publishers; // Đổi tên để khớp với FilterProducts
        }
        // GET: SanPhams/Import
        public ActionResult Import()
        {
            return View();
        }

        // POST: SanPhams/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // Check if the file is an Excel file
                if (file.FileName.EndsWith(".xlsx"))
                {
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        // Get the first worksheet in the Excel file
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Start from row 2 (skip header)
                        {
                            // Read the Excel data (assuming columns are: IDsp, TenSP, MoTa, TheLoai, GiaBan, HinhAnh, IDtg, IDnxb, IDkm, SoLuong, TrangThaiSach)
                            var sanPham = new SanPham
                            {
                                TenSP = worksheet.Cells[row, 1].Text,
                                MoTa = worksheet.Cells[row, 2].Text,
                                IDtl = int.TryParse(worksheet.Cells[row, 3].Text, out var category) ? category : 0,  // Updated line to properly parse the integer value
                                GiaBan = decimal.TryParse(worksheet.Cells[row, 4].Text, out var price) ? price : 0,
                                HinhAnh = worksheet.Cells[row, 5].Text,
                                IDtg = int.TryParse(worksheet.Cells[row, 6].Text, out var tacGiaId) ? tacGiaId : 0,
                                IDnxb = int.TryParse(worksheet.Cells[row, 7].Text, out var nxbId) ? nxbId : 0,
                                IDkm = int.TryParse(worksheet.Cells[row, 8].Text, out var kmId) ? kmId : 0,
                                SoLuong = int.TryParse(worksheet.Cells[row, 9].Text, out var soLuong) ? soLuong : 0,
                                TrangThaiSach = worksheet.Cells[row, 10].Text
                            };

                            // Add the product to the database
                            db.SanPham.Add(sanPham);
                        }

                        db.SaveChanges();
                        TempData["thanhcong"] = "Nhập sách thành công";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["thatbai"] = "Nhập sách không thành công";
                }
            }
            return View();
        }
        public ActionResult ThongKeSanPham()
        {
            // Lấy danh sách tất cả sản phẩm từ cơ sở dữ liệu
            var sanPhams = db.SanPham.ToList();

            // Tổng sách hết hàng (có số lượng bằng 0)
            int tongSachHetHang = sanPhams.Count(sp => sp.SoLuong == 0);

            // Tổng sách còn hàng (có số lượng lớn hơn 0)
            int tongSachConHang = sanPhams.Count(sp => sp.SoLuong > 0);

            // Tổng số lượng tất cả các cuốn sách
            int tongSoLuongSach = sanPhams.Sum(sp => sp.SoLuong);

            // Tổng số lượng các đầu sách (số lượng sản phẩm riêng biệt)
            int tongDauSach = sanPhams.Count();

            // Tổng giá trị tồn kho (tổng giá bán * số lượng cho tất cả sản phẩm còn hàng)
            decimal tongGiaTriTonKho = sanPhams.Where(sp => sp.SoLuong > 0).Sum(sp => sp.GiaBan * sp.SoLuong);
            // Số lượng các sản phẩm có khuyến mãi
            int tongSanPhamKhuyenMai = sanPhams.Count(sp => sp.IDkm > 0);

            // Số lượng các sản phẩm không có khuyến mãi
            int tongSanPhamKhongKhuyenMai = sanPhams.Count(sp => sp.IDkm > 0);

            // Tạo đối tượng ViewBag để truyền thông tin sang View
            ViewBag.TongSachHetHang = tongSachHetHang;
            ViewBag.TongSachConHang = tongSachConHang;
            ViewBag.TongSoLuongSach = tongSoLuongSach;
            ViewBag.TongDauSach = tongDauSach;
            ViewBag.TongGiaTriTonKho = tongGiaTriTonKho;
            ViewBag.TongSanPhamKhuyenMai = tongSanPhamKhuyenMai;
            ViewBag.TongSanPhamKhongKhuyenMai = tongSanPhamKhongKhuyenMai;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveAll()
        {
            try
            {
                // Lấy danh sách tất cả các sản phẩm có trạng thái "Hết hàng"
                var pdhethang = db.SanPham.Where(dh => dh.TrangThaiSach == "Hết hàng").ToList();

                if (pdhethang.Count == 0)
                {
                    TempData["ErrorMessage"] = "Không có sản phẩm nào có trạng thái 'Hết hàng'.";
                    return RedirectToAction("Index");
                }

                foreach (var donHang in pdhethang)
                {
                    db.SanPham.Remove(donHang); // Xóa các sản phẩm hết hàng
                }

                int changes = db.SaveChanges();
                if (changes > 0)
                {
                    TempData["SuccessMessage"] = "Tất cả các sản phẩm hết hàng đã được xóa.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không có thay đổi nào được thực hiện khi xóa.";
                }
            }
            catch (Exception ex)
            {
                // In ra chi tiết lỗi
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa các sản phẩm hết hàng: " + ex.Message;

                // Kiểm tra InnerException (nếu có) để có thông tin chi tiết hơn
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += "<br/>Chi tiết lỗi: " + ex.InnerException.Message;
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult SearchSuggestions(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet); // Trả về một mảng rỗng nếu không có query
            }

            // Loại bỏ khoảng trắng thừa từ chuỗi nhập vào
            query = query.Trim().ToLower();  // Chuyển thành chữ thường để so sánh không phân biệt hoa/thường

            // Tìm kiếm sản phẩm theo tên sách, so sánh không phân biệt chữ hoa chữ thường và bao gồm khoảng trắng
            var suggestionsBox = db.SanPham
                .Where(s => s.TenSP.ToLower().Contains(query))  // Kiểm tra xem tên sản phẩm có chứa chuỗi tìm kiếm
                .Take(10)  // Giới hạn số lượng gợi ý
                .Select(s => new { NameSP = s.TenSP })  // Chọn chỉ tên sách với đúng tên thuộc tính
                .ToList();

            return Json(suggestionsBox, JsonRequestBehavior.AllowGet);  // Trả về danh sách gợi ý
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
