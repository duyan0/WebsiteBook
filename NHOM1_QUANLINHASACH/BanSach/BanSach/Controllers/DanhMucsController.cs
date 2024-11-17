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
        //Factory Method
        private readonly DanhMucService danhMucService;
        public DanhMucsController()
        {
            var db = new dbSach();
            var factory = new DanhMucFactory();
            danhMucService = new DanhMucService(db, factory);
        }

        public ActionResult Create()
        {
            var danhMucList = danhMucService.GetDanhMucList();
            ViewBag.DanhMucList = danhMucList;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DanhMuc1,TheLoai")] DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                if (danhMucService.IsDanhMucExists(danhMuc.DanhMuc1, danhMuc.TheLoai))
                {
                    ModelState.AddModelError("", "Danh mục và thể loại đã tồn tại. Vui lòng nhập danh mục khác.");
                    return View(danhMuc);
                }

                danhMucService.AddDanhMuc(danhMuc.ID, danhMuc.DanhMuc1, danhMuc.TheLoai);
                return RedirectToAction("Index");
            }

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


