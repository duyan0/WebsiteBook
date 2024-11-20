using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BanSach.Models;

namespace BanSach.Controllers
{
    public class DanhMucsController : Controller
    {
        private dbSach db = new dbSach();

        // Action PartialViewResult
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult PhanDanhMuc()
        {
            var cateList = db.DanhMuc.ToList();
            return PartialView(cateList);
        }
        public ActionResult Index()
        {
            return View(db.DanhMuc.ToList());
        }

        // GET: DanhMucs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMuc danhMuc = db.DanhMuc.Find(id);
            if (danhMuc == null)
            {
                return HttpNotFound();
            }
            return View(danhMuc);
        }
        // Factory Method: Sử dụng DanhMucService để thao tác với các đối tượng DanhMuc
        private readonly DanhMucService danhMucService;

        // Constructor cho DanhMucsController
        public DanhMucsController()
        {
            // Khởi tạo db context và Factory để tạo đối tượng DanhMucService
            var db = new dbSach();  // dbSach đại diện cho cơ sở dữ liệu
            var factory = new DanhMucFactory();  // Factory để tạo đối tượng DanhMuc
            danhMucService = new DanhMucService(db, factory);  // Tạo instance của DanhMucService
        }

        // Phương thức Create (GET): Tạo một View để hiển thị danh sách DanhMuc cho người dùng
        public ActionResult Create()
        {
            // Lấy danh sách các tên danh mục từ DanhMucService
            var danhMucList = danhMucService.GetDanhMucList();

            // Gán danh sách danh mục vào ViewBag để sử dụng trong View
            ViewBag.DanhMucList = danhMucList;

            return View();  // Trả về View cho người dùng điền thông tin để tạo danh mục mới
        }

        // Phương thức Create (POST): Nhận dữ liệu từ người dùng để thêm một danh mục mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DanhMuc1,TheLoai")] DanhMuc danhMuc)
        {
            // Kiểm tra xem dữ liệu nhập vào có hợp lệ hay không
            if (ModelState.IsValid)
            {
                // Kiểm tra xem danh mục với tên và thể loại đã tồn tại chưa
                if (danhMucService.IsDanhMucExists(danhMuc.DanhMuc1, danhMuc.TheLoai))
                {
                    // Nếu danh mục đã tồn tại, thêm lỗi vào ModelState để hiển thị cho người dùng
                    ModelState.AddModelError("", "Thể loại này đã tồn tại");
                    return View(danhMuc);  // Trả lại View với thông tin đã nhập cùng với thông báo lỗi
                }

                // Nếu danh mục chưa tồn tại, thêm vào cơ sở dữ liệu
                danhMucService.AddDanhMuc(danhMuc.ID, danhMuc.DanhMuc1, danhMuc.TheLoai);

                // Sau khi thêm thành công, chuyển hướng về trang Index để xem danh sách danh mục
                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, trả lại View với thông tin đã nhập
            return View(danhMuc);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMuc danhMuc = db.DanhMuc.Find(id);
            if (danhMuc == null)
            {
                return HttpNotFound();
            }
            return View(danhMuc);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DanhMuc1,TheLoai")] DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(danhMuc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(danhMuc);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMuc danhMuc = db.DanhMuc.Find(id);
            if (danhMuc == null)
            {
                return HttpNotFound();
            }
            return View(danhMuc);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Tìm tất cả sản phẩm liên quan và xóa chúng
            var relatedProducts = db.SanPham.Where(sp => sp.TheLoai == id);
            foreach (var product in relatedProducts)
            {
                db.SanPham.Remove(product);
            }

            // Xóa danh mục sau khi sản phẩm liên quan đã được xóa
            DanhMuc danhMuc = db.DanhMuc.Find(id);
            db.DanhMuc.Remove(danhMuc);
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


