using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using BanSach.Models;

namespace BanSach.Controllers
{
    public class DonHangCTsController : Controller
    {
        private SachEntities1 db = new SachEntities1();

        // GET: DonHangCTs
        public ActionResult Index()
        {
            var donHangCTs = db.DonHangCT.Include(d => d.DonHang).Include(d => d.SanPham);
            return View(donHangCTs.ToList());
        }

        // GET: DonHangCTs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHangCT donHangCT = db.DonHangCT.Find(id);
            if (donHangCT == null)
            {
                return HttpNotFound();
            }
            return View(donHangCT);
        }

        // GET: DonHangCTs/Create
        public ActionResult Create()
        {
            ViewBag.IDDonHang = new SelectList(db.DonHang, "IDdh", "DiaChi");
            ViewBag.IDSanPham = new SelectList(db.SanPham, "IDsp", "TenSP");
            return View();
        }

        // POST: DonHangCTs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDdh,IDSanPham,IDDonHang,SoLuong,Gia,DanhGia")] DonHangCT donHangCT)
        {
            if (ModelState.IsValid)
            {
                db.DonHangCT.Add(donHangCT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDDonHang = new SelectList(db.DonHang, "IDdh", "DiaChi", donHangCT.IDDonHang);
            ViewBag.IDSanPham = new SelectList(db.SanPham, "IDsp", "TenSP", donHangCT.IDSanPham);
            return View(donHangCT);
        }

        // GET: DonHangCTs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHangCT donHangCT = db.DonHangCT.Find(id);
            if (donHangCT == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDDonHang = new SelectList(db.DonHang, "IDdh", "DiaChi", donHangCT.IDDonHang);
            ViewBag.IDSanPham = new SelectList(db.SanPham, "IDsp", "TenSP", donHangCT.IDSanPham);
            return View(donHangCT);
        }

        // POST: DonHangCTs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDdh,IDSanPham,IDDonHang,SoLuong,Gia,DanhGia")] DonHangCT donHangCT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donHangCT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDDonHang = new SelectList(db.DonHang, "IDdh", "DiaChi", donHangCT.IDDonHang);
            ViewBag.IDSanPham = new SelectList(db.SanPham, "IDsp", "TenSP", donHangCT.IDSanPham);
            return View(donHangCT);
        }

        // GET: DonHangCTs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHangCT donHangCT = db.DonHangCT.Find(id);
            if (donHangCT == null)
            {
                return HttpNotFound();
            }
            return View(donHangCT);
        }

        // POST: DonHangCTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DonHangCT donHangCT = db.DonHangCT.Find(id);
            db.DonHangCT.Remove(donHangCT);
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
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [ChildActionOnly]
        public PartialViewResult TopBanChay()
        {
            List<DonHangCT> orderD = db.DonHangCT.ToList();
            List<SanPham> prolist = db.SanPham.ToList();
            var query = from od in orderD join p in prolist on od.IDSanPham equals p.IDsp into tbl
                        group od by new { idPro = od.IDSanPham,
                            namePro = od.SanPham.TenSP,
                            imgPro = od.SanPham.HinhAnh,
                            price = od.SanPham.GiaBan 
                        } into gr
                        orderby gr.Sum(s => s.SoLuong) descending
                        select new ViewModel
                        {
                            IdPro = gr.Key.idPro,
                            NamePro = gr.Key.namePro,
                            ImgPro = gr.Key.imgPro,
                            price = (decimal)gr.Key.price,
                            Sum_Quantity = gr.Sum(s => s.SoLuong)
                        };


            return PartialView(query.Take(10).ToList());
        }
    }
}
