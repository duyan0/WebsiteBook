using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StatePattern
{
    public class DeliveredState : IOrderState
    {
        public void UpdateStatus(DonHang donHang, db_Book db)
        {
            throw new InvalidOperationException("Đơn hàng đã được giao, không thể thay đổi trạng thái.");
        }

        public string GetStatus() => "Đã nhận hàng";
    }
}