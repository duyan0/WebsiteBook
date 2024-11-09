using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Admin
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, MinimumLength = 9, ErrorMessage = "Số điện thoại phải có từ 9 đến 15 ký tự")]
        public string SoDT { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
        [StringLength(50, ErrorMessage = "Vai trò không được vượt quá 50 ký tự")]
        public string VaiTro { get; set; }

        [Required(ErrorMessage = "Tài khoản không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên tài khoản phải có từ 3 đến 50 ký tự")]
        public string TKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string MKhau { get; set; }
    }
}
