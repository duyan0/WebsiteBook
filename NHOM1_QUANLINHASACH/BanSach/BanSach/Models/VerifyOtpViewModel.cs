using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class VerifyOtpViewModel
    {
        public int UserId { get; set; }
        public string EnteredOtp { get; set; }
    }
}