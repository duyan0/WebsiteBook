using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StrategyPattern
{
    // Strategy cho RequestReturn
    public class RequestReturnStrategy : IOrderStatusStrategy
    {
        public void UpdateStatus(DonHang donHang, db_Book db)
        {
            donHang.TrangThai = "Yêu cầu đổi trả";
            db.Entry(donHang).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
    }
}