using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;

    public partial class KhuyenMai
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhuyenMai()
        {
            this.SanPham = new HashSet<SanPham>();
        }
        public int IDkm { get; set; }

        [Required(ErrorMessage = "Tên khuyến mãi không được để trống")]
        [StringLength(100, ErrorMessage = "Tên khuyến mãi không được vượt quá 100 ký tự")]
        public string TenKhuyenMai { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.Date, ErrorMessage = "Ngày bắt đầu không hợp lệ")]
        public Nullable<System.DateTime> NgayBatDau { get; set; }

        
        [CustomValidation(typeof(KhuyenMai), "ValidateNgayKetThuc")]
        public Nullable<System.DateTime> NgayKetThuc { get; set; }

        [Required(ErrorMessage = "Mức giảm giá không được để trống")]
        [Range(0, 100, ErrorMessage = "Mức giảm giá phải từ 0 đến 100%")]
        public Nullable<int> MucGiamGia { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string MoTa { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ngày tạo không hợp lệ")]
        public Nullable<System.DateTime> NgayTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPham { get; set; }
        public static ValidationResult ValidateNgayKetThuc(DateTime? ngayKetThuc, ValidationContext context)
        {
            var instance = context.ObjectInstance as KhuyenMai;
            if (instance == null) return ValidationResult.Success;

            if (instance.NgayBatDau.HasValue && ngayKetThuc.HasValue && ngayKetThuc < instance.NgayBatDau)
            {
                return new ValidationResult("Ngày kết thúc phải sau ngày bắt đầu.");
            }

            return ValidationResult.Success;
        }
    }
}
