using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class PromotionViewModel
    {
        public int IDkm { get; set; }
        public string TenKhuyenMai { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int MucGiamGia { get; set; }
        public string MoTa { get; set; }
        public int SanPhamCount { get; set; }  // Custom field for product count
    }
}