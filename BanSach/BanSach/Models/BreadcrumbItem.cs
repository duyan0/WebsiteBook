using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class BreadcrumbItem
    {
        public string Text { get; set; } // Tên hiển thị
        public string Url { get; set; }  // Đường dẫn
        public bool IsActive { get; set; } // Đánh dấu mục hiện tại
    }
}