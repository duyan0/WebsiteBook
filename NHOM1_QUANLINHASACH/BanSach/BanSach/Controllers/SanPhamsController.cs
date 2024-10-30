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
        private SachEntities1 db = new SachEntities1();

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
            
            int pageSize = 8;           
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

        // GET: SanPhams/Create
        public ActionResult Create()
        {
            ViewBag.TheLoai = new SelectList(db.DanhMuc, "ID", "TheLoai");
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTacGia");
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb");
            return View();
        }

        // POST: SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDsp,TenSP,MoTa,TheLoai,GiaBan,HinhAnh,IDtg,IDnxb,NamXB,SoLuong,TrangThaiSach")] SanPham sanPham)
        {           
            if (ModelState.IsValid)
            {
                db.SanPham.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TheLoai = new SelectList(db.DanhMuc, "ID", "TheLoai", sanPham.TheLoai);
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTacGia", sanPham.TacGia);
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb",sanPham.NhaXuatBan);
            return View(sanPham);
        }

        // GET: SanPhams/Edit/5
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
            ViewBag.TheLoai = new SelectList(db.DanhMuc, "ID", "TheLoai", product.TheLoai);
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTacGia", product.TacGia);
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb", product.NhaXuatBan);
            return View(product);
        }

        // POST: SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDsp,TenSP,MoTa,TheLoai,GiaBan,HinhAnh,IDtg,IDnxb,NamXB,SoLuong,TrangThaiSach")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {                
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TheLoai = new SelectList(db.DanhMuc, "ID", "TheLoai", sanPham.TheLoai);
            ViewBag.TacGia = new SelectList(db.TacGia, "IDtg", "TenTacGia", sanPham.TacGia);
            ViewBag.NXB = new SelectList(db.NhaXuatBan, "IDnxb", "Tennxb", sanPham.NhaXuatBan);
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
            db.SanPham.Remove(sanPham);
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
}
