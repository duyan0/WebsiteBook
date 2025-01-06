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
        private readonly db_book1 db = new db_book1();
        public ActionResult Index()
        {
            var list = db.DanhMuc_TheLoai.ToList();
            return View(list);
        }
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
                db.DanhMuc_TheLoai.Add(dm_tl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai", dm_tl.TheLoai);
            ViewBag.DM = new SelectList(db.DanhMuc, "ID", "TenDanhMuc", dm_tl.DanhMuc);

            return View(dm_tl);
        }
        public ActionResult Edit()
        {
            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai");
            ViewBag.DM = new SelectList(db.DanhMuc, "ID", "TenDanhMuc");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DanhMuc_TheLoai model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TL = new SelectList(db.TheLoai, "ID", "TenTheLoai", model.TheLoai);
            ViewBag.DM = new SelectList(db.DanhMuc, "ID", "TenDanhMuc", model.DanhMuc);
            return View(model);
        }
    }
}