using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StatePattern
{
    public class ConfirmedState : IOrderState
    {
        public void UpdateStatus(DonHang donHang, db_Book db)
        {
            System.Diagnostics.Debug.WriteLine($"ConfirmedState.UpdateStatus - Trước: TrangThai: {donHang.TrangThai}");

            donHang.TrangThai = "Đã nhận hàng";
            donHang.State = new DeliveredState();
            donHang.NgayNhanHang = DateTime.Now; // Cập nhật ngày nhận hàng
            db.Entry(donHang).State = EntityState.Modified;
            int rowsAffected = db.SaveChanges();

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException("Không thể cập nhật trạng thái đơn hàng vào cơ sở dữ liệu.");
            }

            System.Diagnostics.Debug.WriteLine($"ConfirmedState.UpdateStatus - Sau: TrangThai: {donHang.TrangThai}, Rows Affected: {rowsAffected}");
        }

        public string GetStatus() => "Đã xác nhận";
    }
}