using System;
using System.Collections.Generic;
using System.Linq;

namespace BanSach.Models
{
    // DanhMucService class cung cấp các phương thức dịch vụ để thao tác với danh mục
    public class DanhMucService
    {
        // Các trường readonly cho cơ sở dữ liệu (db) và Factory để tạo đối tượng DanhMuc
        private readonly db_book1 db;  // Kết nối đến cơ sở dữ liệu dbSach
        private readonly DanhMucFactory danhMucFactory;  // Factory để tạo đối tượng DanhMuc

        // Constructor khởi tạo với hai tham số db context và Factory
        public DanhMucService(db_book1 context, DanhMucFactory factory)
        {
            db = context;
            danhMucFactory = factory;
        }

        // Phương thức GetDanhMucList trả về danh sách tên của các danh mục
        public List<string> GetDanhMucList()
        {
            // Truy vấn vào cơ sở dữ liệu để lấy danh sách tên DanhMuc (DanhMuc1)
            return db.DanhMuc.Select(d => d.DanhMuc1).ToList();
        }

        // Phương thức IsDanhMucExists kiểm tra xem một danh mục với tên và thể loại đã tồn tại chưa
        public bool IsDanhMucExists(string danhMuc1, string theLoai)
        {
            // Sử dụng LINQ để kiểm tra xem có bất kỳ danh mục nào khớp với tên và thể loại được truyền vào
            return db.DanhMuc.Any(d => d.DanhMuc1 == danhMuc1 && d.TheLoai == theLoai);
        }

        // Phương thức AddDanhMuc để thêm một danh mục mới vào cơ sở dữ liệu
        public void AddDanhMuc(int id, string danhMuc1, string theLoai)
        {
            // Sử dụng Factory để tạo đối tượng DanhMuc mới với các thông tin được cung cấp
            var danhMuc = danhMucFactory.CreateDanhMuc(id, danhMuc1, theLoai);

            // Thêm đối tượng danhMuc vào db context và lưu thay đổi vào cơ sở dữ liệu
            db.DanhMuc.Add(danhMuc);
            db.SaveChanges();
        }
    }
}
