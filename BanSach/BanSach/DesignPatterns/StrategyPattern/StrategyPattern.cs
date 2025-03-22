using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StrategyPattern
{
    public interface IOrderStatusStrategy
    {
        void UpdateStatus(DonHang donHang, db_Book db);
    }
}