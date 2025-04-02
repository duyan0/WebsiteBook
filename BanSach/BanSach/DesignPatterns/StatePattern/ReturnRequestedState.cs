using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StatePattern
{
    public class ReturnRequestedState : IOrderState
    {
        public void UpdateStatus(DonHang donHang, db_Book db)
        {
            // Chuyển sang trạng thái "Đã hủy"
            donHang.TrangThai = "Đã huỷ";
            donHang.State = new CancelledState();
            db.Entry(donHang).State = EntityState.Modified;
            db.SaveChanges();
        }

        public string GetStatus() => "Yêu cầu trả hàng";
    }
}