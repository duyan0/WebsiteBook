using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace BanSach.Models
{
    public partial class KhuyenMai
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhuyenMai()
        {
            this.SanPham = new HashSet<SanPham>();
        }

        public int IDkm { get; set; }

        [DisplayName("Tên Khuyến Mãi")]
        [Required(ErrorMessage = "Tên khuyến mãi không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên khuyến mãi không được vượt quá 200 ký tự.")]
        public string TenKhuyenMai { get; set; }

        [DisplayName("Ngày Bắt Đầu")]
        [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
        [DataType(DataType.Date, ErrorMessage = "Ngày bắt đầu không hợp lệ.")]
        public Nullable<System.DateTime> NgayBatDau { get; set; }

        [DisplayName("Ngày Kết Thúc")]
        public Nullable<System.DateTime> NgayKetThuc { get; set; }

        [DisplayName("Mức Giảm Giá (%)")]
        [Required(ErrorMessage = "Mức giảm giá không được để trống.")]
        [Range(0, 100, ErrorMessage = "Mức giảm giá phải nằm trong khoảng từ 0 đến 100%.")]
        public Nullable<decimal> MucGiamGia { get; set; }

        [DisplayName("Mô Tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string MoTa { get; set; }

        [DisplayName("Ngày Tạo")]
        public Nullable<System.DateTime> NgayTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPham { get; set; }
    }
}