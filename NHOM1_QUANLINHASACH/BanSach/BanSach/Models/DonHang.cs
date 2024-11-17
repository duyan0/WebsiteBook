namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public partial class DonHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonHang()
        {
            this.DonHangCT = new HashSet<DonHangCT>();
            this.NgayDatHang = DateTime.Now; // Gán giá trị ngày hiện tại
            this.NgayNhanHang = DateTime.Now;
            // Có thể gán NgayNhanHang sau này khi nhận hàng
        }

        public int IDdh { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NgayDatHang { get; set; }
        public Nullable<int> IDkh { get; set; }
        public string DiaChi { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NgayNhanHang { get; set; }
        public string TrangThai { get; set; } = "Chờ xử lý";
        public virtual KhachHang KhachHang { get; set; }
        public virtual ICollection<DonHangCT> DonHangCT { get; set; }

        public decimal GetTongSoTien()
        {
            return DonHangCT != null
                ? DonHangCT.Sum(ct => (ct.SoLuong ?? 0) * (decimal)(ct.Gia ?? 0))
                : 0;
        }

    }
}
