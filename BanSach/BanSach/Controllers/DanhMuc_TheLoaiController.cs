using BanSach.Models;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class DanhMuc_TheLoaiController : Controller
    {
        private readonly db_Book db = new db_Book();

        // GET: DanhMuc_TheLoai
        public ActionResult Index()
        {
            var list = db.DanhMuc_TheLoai
                .Include(d => d.DanhMuc)
                .Include(d => d.TheLoai)
                .ToList();
            return View(list);
        }

        // GET: DanhMuc_TheLoai/Create
        public ActionResult Create()
        {
            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai");
            ViewBag.DM = new SelectList(db.DanhMuc, "ID", "TenDanhMuc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DanhMuc_TheLoai dm_tl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.DanhMuc_TheLoai.Add(dm_tl);
                    db.SaveChanges();
                    TempData["Success"] = "Thêm mới thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm mới: " + ex.Message);
                }
            }
            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai", dm_tl.TheLoai);
            ViewBag.DM = new SelectList(db.DanhMuc, "ID", "TenDanhMuc", dm_tl.DanhMuc);
            return View(dm_tl);
        }

        // GET: DanhMuc_TheLoai/Edit/ID
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DanhMuc_TheLoai dm_tl = db.DanhMuc_TheLoai.Find(id);
            if (dm_tl == null)
            {
                return HttpNotFound();
            }
            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai", dm_tl.TheLoai);
            ViewBag.DM = new SelectList(db.DanhMuc, "ID", "TenDanhMuc", dm_tl.DanhMuc);
            return View(dm_tl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DanhMuc_TheLoai model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);
                }
            }
            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai", model.TheLoai);
            ViewBag.DM = new SelectList(db.DanhMuc, "ID", "TenDanhMuc", model.DanhMuc);
            return View(model);
        }

        // GET: DanhMuc_TheLoai/Delete/ID
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DanhMuc_TheLoai dm_tl = db.DanhMuc_TheLoai.Find(id);
            if (dm_tl == null)
            {
                return HttpNotFound();
            }
            return View(dm_tl);
        }

        // POST: DanhMuc_TheLoai/Delete/ID
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                DanhMuc_TheLoai dm_tl = db.DanhMuc_TheLoai.Find(id);
                if (dm_tl == null)
                {
                    return HttpNotFound();
                }
                db.DanhMuc_TheLoai.Remove(dm_tl);
                db.SaveChanges();
                TempData["Success"] = "Xóa thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: DanhMuc_TheLoai/Details/ID
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DanhMuc_TheLoai dm_tl = db.DanhMuc_TheLoai
                .Include(d => d.DanhMuc)
                .Include(d => d.TheLoai)
                .FirstOrDefault(d => d.ID == id);
            if (dm_tl == null)
            {
                return HttpNotFound();
            }
            return View(dm_tl);
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