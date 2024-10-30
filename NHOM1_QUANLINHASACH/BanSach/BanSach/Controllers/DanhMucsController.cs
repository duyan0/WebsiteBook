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
        private SachEntities1 db = new SachEntities1();

        // Action PartialViewResult
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult PhanDanhMuc()
        {
            var cateList = db.DanhMuc.ToList();
            return PartialView(cateList);
        }

        // GET: DanhMucs
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

        // GET: DanhMucs/Create
        public ActionResult Create()
        {
            var danhMucList = db.DanhMuc.Select(d => d.DanhMuc1).ToList();
            ViewBag.DanhMucList = danhMucList;

            return View();
        }

        // POST: DanhMucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DanhMuc1,TheLoai")] DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem danh mục và thể loại đã tồn tại chưa
                var existingCategory = db.DanhMuc
                    .FirstOrDefault(d => d.DanhMuc1 == danhMuc.DanhMuc1 && d.TheLoai == danhMuc.TheLoai);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("", "Danh mục và thể loại đã tồn tại. Vui lòng nhập danh mục khác.");
                    return View(danhMuc);
                }

                db.DanhMuc.Add(danhMuc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(danhMuc);
        }



        // GET: DanhMucs/Edit/5
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

        // POST: DanhMuc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: DanhMuc/Delete/5
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

        // POST: DanhMuc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
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
