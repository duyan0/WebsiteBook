using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BanSach.Controllers
{
    public class NhaXuatBanController : Controller
    {
        // GET: NhaXuatBan
        SachEntities1 db = new SachEntities1();
        public ActionResult Index()
        {
            return View(db.NhaXuatBan.ToList());
        }
        public ActionResult Create()
        {
            var nxb = db.NhaXuatBan.ToList();
            if (nxb != null && nxb.Count > 0)
            {
                ViewBag.NXB = new SelectList(nxb, "IDnxb", "Tennxb");
            }
            else
            {
                ViewBag.NXB = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhaXuatBan model) // Đổi ModelType với tên model bạn đang sử dụng
        {
            if (ModelState.IsValid)
            {
                // Lưu model vào cơ sở dữ liệu
                db.NhaXuatBan.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hãy gọi lại danh sách tác giả
            var nxb = db.NhaXuatBan.ToList();
            ViewBag.NXB = new SelectList(nxb, "IDnxb", "Tennxb", model.IDnxb); // Ghi lại chọn IDtg hiện tại
            return View(model);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NhaXuatBan tg = db.NhaXuatBan.Find(id);  // Tìm tác giả dựa trên ID
            if (tg == null)
            {
                return HttpNotFound();
            }

            return View(tg);  // Truyền model TacGia vào View để hiển thị
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDnxb,Tennxb,DiaChi,SoDienThoai,Email")] NhaXuatBan nxb)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nxb).State = EntityState.Modified;  // Đánh dấu đối tượng là đã sửa đổi
                db.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction("Index");  // Chuyển hướng về trang danh sách sau khi cập nhật thành công
            }

            return View(nxb);  // Nếu không hợp lệ, trả lại form chỉnh sửa với dữ liệu hiện có
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);  // Trả về lỗi nếu ID không hợp lệ
            }

            NhaXuatBan nxb = db.NhaXuatBan.Find(id);  // Tìm đối tượng TacGia dựa vào ID
            if (nxb == null)
            {
                return HttpNotFound();  // Nếu không tìm thấy, trả về lỗi 404
            }

            return View(nxb);  // Trả về view và truyền model TacGia vào view để hiển thị chi tiết
        }
        // GET: TacGia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NhaXuatBan nxb = db.NhaXuatBan.Find(id);
            if (nxb == null)
            {
                return HttpNotFound();
            }

            return View(nxb);
        }
        // POST: TacGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NhaXuatBan nxb = db.NhaXuatBan.Find(id);
            db.NhaXuatBan.Remove(nxb);
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