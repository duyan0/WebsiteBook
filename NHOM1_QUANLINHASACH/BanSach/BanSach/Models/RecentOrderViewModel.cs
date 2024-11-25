using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class RecentOrderViewModel
    {
        public int Id { get; set; }
        public string TenSanPham { get; set; }
        public decimal Gia { get; set; }
        public DateTime NgayDatHang { get; set; }
        public string TrangThai { get; set; }
        public string HinhAnh { get; set; }
    }
}