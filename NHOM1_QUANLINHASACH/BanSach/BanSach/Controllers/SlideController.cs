using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class SlideController : Controller
    {
        private readonly db_book1 db = new db_book1();
        public ActionResult Index()
        {
            var slides = db.Slide.ToList();
            return View(slides);
        }
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult Silde()
        {
            var slide = db.Slide.ToList();
            return PartialView(slide);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Banner/Create (Thực hiện thêm mới banner)
        [HttpPost]
        public ActionResult Create(Slide Slide)
        {
            if (ModelState.IsValid)
            {
                db.Slide.Add(Slide);
                db.SaveChanges();
                return RedirectToAction("Index");  // Điều hướng lại về danh sách banner
            }
            return View(Slide);
        }

        // GET: Banner/Edit/{id} (Hiển thị form sửa banner)
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var banner = db.Slide.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
        }

        // POST: Banner/Edit/{id} (Thực hiện sửa banner)
        [HttpPost]
        public ActionResult Edit(Slide Slide)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Slide).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");  // Điều hướng lại về danh sách banner
            }
            return View(Slide);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var banner = db.Slide.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }

            db.Slide.Remove(banner);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}