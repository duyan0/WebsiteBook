using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class ThongKeDanhMucViewModel
    {
        // Tổng số danh mục
        public int TotalCategories { get; set; }

        // Tổng số sách trong tất cả các danh mục
        public int TotalBooks { get; set; }

        // Danh sách sách theo từng danh mục
        public List<SachTheoDanhMuc> BooksPerCategory { get; set; }
    }

    // Lớp dùng để thống kê số lượng sách theo từng danh mục
    public class SachTheoDanhMuc
    {
        public string DanhMuc { get; set; }
        public int SoLuongSach { get; set; }
    }
}
