using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StatePattern
{
    public interface IOrderState
    {
        void UpdateStatus(DonHang donHang, db_Book db);
        string GetStatus();
    }
}