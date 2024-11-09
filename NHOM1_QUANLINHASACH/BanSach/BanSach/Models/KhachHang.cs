using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;

    public partial class KhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhachHang()
        {
            this.DonHang = new HashSet<DonHang>();
        }

        public int IDkh { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        [StringLength(40, ErrorMessage = "Tên khách hàng không được vượt quá 40 ký tự")]
        [RegularExpression(@"^[^<>/\\.'"";:{}[\]?!@#$%^&*~`]+$", ErrorMessage = "Tên khách hàng không được chứa các ký tự đặc biệt như < > / \\ . '")]
        public string TenKH { get; set; }


        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, MinimumLength = 9, ErrorMessage = "Số điện thoại phải có từ 9 đến 15 ký tự")]
        public string SoDT { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tài khoản không được để trống")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Tên tài khoản phải có từ 5 đến 50 ký tự")]
        public string TKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        public string MKhau { get; set; }

        [NotMapped] 
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("MKhau", ErrorMessage = "Mật khẩu xác nhận không khớp với mật khẩu đã nhập")]
        [DataType(DataType.Password)]
        public string ConfirmPass { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHang> DonHang { get; set; }
    }
}
