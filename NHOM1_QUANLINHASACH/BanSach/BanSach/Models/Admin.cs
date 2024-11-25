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
        [RegularExpression(@"^[A-Za-zÀ-ỹ\s]+$", ErrorMessage = "Họ tên chỉ được chứa các ký tự chữ và dấu cách.")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [StringLength(200, MinimumLength = 20, ErrorMessage = "Địa chỉ không dưới 20 và không quá 200 kí tự.")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập 10 hoặc 11 chữ số.")]
        [StringLength(11, ErrorMessage = "Số điện thoại không được vượt quá 11 ký tự.")]
        public string SoDT { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
        [StringLength(50, ErrorMessage = "Vai trò không được vượt quá 50 ký tự")]
        public string VaiTro { get; set; }

        [Required(ErrorMessage = "Tài khoản không được để trống")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Tên tài khoản phải có từ 8 đến 50 ký tự")]
        [RegularExpression(@"^[a-z0-9]+$", ErrorMessage = "Tài khoản chỉ được chứa chữ cái thường và số, không có dấu.")]
        public string TKhoan { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,20}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường, chữ số và ký tự đặc biệt.")]
        public string MKhau { get; set; }
    }
}