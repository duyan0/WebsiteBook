using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu mới không được để trống.")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.")]
        public string ConfirmNewPassword { get; set; }
    }

}