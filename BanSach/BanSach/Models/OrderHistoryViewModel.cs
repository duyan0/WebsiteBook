using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class OrderHistoryViewModel
    {
        public IPagedList<DonHang> ConfirmedOrders { get; set; }
        public IPagedList<DonHang> PendingOrders { get; set; }
        public IPagedList<DonHang> CanceledOrders { get; set; }
        public IPagedList<DonHang> ReceivedOrders { get; set; }
    }
}