using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class ViewModel
    {
        public string NamePro { get; set; }
        public string DesPro { get; set; }
        public string ImgPro { get; set; }
        public decimal price { get; set; }
        public string NameCate { get; set; }
        [System.ComponentModel.DataAnnotations.Key]
        public int? IdPro { get; set; }
        public decimal Total_Money { get; set; }

        public SanPham product { get; set; }
        public DanhMuc category { get; set; }
        public DonHangCT orderDetail { get; set; }
        public SanPham Status { get; set; }
        public KhuyenMai giaKM { get; set; }
        public IEnumerable<SanPham> ListProduct { get; set; }
        public int? Top5_Quantity { get; set; }
        public int? Sum_Quantity { get; set; }
    }
}