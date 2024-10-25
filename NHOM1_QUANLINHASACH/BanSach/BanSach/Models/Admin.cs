namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Admin
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Họ tên không được vượt quá 50 ký tự.")]
        [RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỂẾỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s]+$", ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng.")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email phải có định dạng '@gmail.com'.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Địa chỉ không được vượt quá 100 ký tự.")]
       
        public string DiaChi { get; set; }


        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^(\+84|0)\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SoDT { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc.")]
        [RegularExpression(@"^(Nhân viên|Admin)$", ErrorMessage = "Vai trò phải là 'Nhân viên' hoặc 'Admin'.")]
        public string VaiTro { get; set; }

        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Tài khoản phải có từ 5 đến 20 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tài khoản không được chứa ký tự đặc biệt, khoảng trắng hoặc ký tự tiếng Việt.")]
        public string TKhoan { get; set; }


        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tài khoản không được chứa ký tự đặc biệt, khoảng trắng hoặc ký tự tiếng Việt.")]
        public string MKhau { get; set; }
    }
}
