namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class NhaXuatBan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhaXuatBan()
        {
            this.SanPham = new HashSet<SanPham>();
        }

        public int IDnxb { get; set; }

        [Display(Name = "Tên Nhà xuất bản")]
        [Required(ErrorMessage = "Tên nhà xuất bản là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên nhà xuất bản không được vượt quá 100 ký tự.")]
        public string Tennxb { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
        public string DiaChi { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^(\+84|0)[1-9]\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại hợp lệ.")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPham { get; set; }
    }
}
