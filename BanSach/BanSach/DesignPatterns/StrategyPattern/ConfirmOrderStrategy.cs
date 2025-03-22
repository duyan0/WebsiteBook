using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StrategyPattern
{
    public class ConfirmOrderStrategy : IOrderStatusStrategy
    {
        public void UpdateStatus(DonHang donHang, db_Book db)
        {
            donHang.TrangThai = "Đã xác nhận";
            db.Entry(donHang).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
    }
}