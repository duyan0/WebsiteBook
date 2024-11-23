using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;

    public partial class TacGia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TacGia()
        {
            this.SanPham = new HashSet<SanPham>();
        }

        public int IDtg { get; set; }

        [Required(ErrorMessage = "Tên tác giả không được để trống")]
        [StringLength(150, ErrorMessage = "Tên tác giả không được vượt quá 150 ký tự")]
        public string TenTacGia { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ngày sinh không hợp lệ")]
        [CustomValidation(typeof(TacGia), nameof(ValidateNgaySinh))]
        public Nullable<System.DateTime> NgaySinh { get; set; }

        [Required(ErrorMessage = "Quốc gia không được để trống")]
        [StringLength(100, ErrorMessage = "Tên quốc gia không được vượt quá 100 ký tự")]
        public string QuocGia { get; set; }

        [StringLength(250, ErrorMessage = "Tiểu sử không được vượt quá 250 ký tự")]
        public string TieuSu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPham { get; set; }
        public static ValidationResult ValidateNgaySinh(DateTime? ngaySinh, ValidationContext context)
        {
            if (ngaySinh.HasValue && ngaySinh > DateTime.Now)
            {
                return new ValidationResult("Ngày sinh không thể lớn hơn ngày hiện tại.");
            }
            return ValidationResult.Success;
        }
    }
}