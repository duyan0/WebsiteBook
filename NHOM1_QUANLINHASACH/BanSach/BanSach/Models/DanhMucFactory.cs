//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace BanSach.Models
//{
//    // DanhMucFactory class sử dụng Factory Pattern để tạo các đối tượng DanhMuc
//    public class DanhMucFactory
//    {
//        // Phương thức CreateDanhMuc giúp tạo ra một đối tượng DanhMuc với các thông tin được cung cấp
//        public DanhMuc CreateDanhMuc(int id, string danhMuc1, string theLoai)
//        {
//            // Trả về đối tượng DanhMuc với các thuộc tính được khởi tạo bằng tham số truyền vào
//            return new DanhMuc
//            {
//                // ID của danh mục
//                ID = id,

//                // Tên danh mục - Nên cân nhắc đổi tên thuộc tính này thành TenDanhMuc để dễ hiểu hơn
//                DanhMuc1 = danhMuc1,

//                // Thể loại của danh mục
//                TheLoai = theLoai
//            };
//        }
//    }

//    // Note: Để đoạn code hoạt động đúng, cần định nghĩa class DanhMuc với các thuộc tính: ID, DanhMuc1, TheLoai
//    // Ví dụ:
//    // public class DanhMuc
//    // {
//    //     public int ID { get; set; }
//    //     public string DanhMuc1 { get; set; }
//    //     public string TheLoai { get; set; }
//    // }
//}
