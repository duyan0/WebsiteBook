using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StrategyPattern
{
    // Strategy cho Cancel
    public class CancelOrderStrategy : IOrderStatusStrategy
    {
        public void UpdateStatus(DonHang donHang, db_Book db)
        {
            donHang.TrangThai = "Đã huỷ";
            db.Entry(donHang).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
    }
}