using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StrategyPattern
{
    // Strategy cho DaNhanHang
    public class ReceivedOrderStrategy : IOrderStatusStrategy
    {
        public void UpdateStatus(DonHang donHang, db_Book db)
        {
            donHang.NgayNhanHang = DateTime.Now;
            donHang.TrangThai = "Đã nhận hàng";
            db.Entry(donHang).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
    }
}