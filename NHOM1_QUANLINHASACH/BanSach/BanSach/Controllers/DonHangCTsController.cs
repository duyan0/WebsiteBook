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
        private dbSach db = new dbSach();

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
        public ActionResult DetailsKH(int? id)
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
