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

        
        public string TenKhuyenMai { get; set; }

     
        public Nullable<System.DateTime> NgayBatDau { get; set; }

        
        [CustomValidation(typeof(KhuyenMai), "ValidateNgayKetThuc")]
        public Nullable<System.DateTime> NgayKetThuc { get; set; }


        public Nullable<int> MucGiamGia { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string MoTa { get; set; }

        [DataType(DataType.DateTime)]
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
