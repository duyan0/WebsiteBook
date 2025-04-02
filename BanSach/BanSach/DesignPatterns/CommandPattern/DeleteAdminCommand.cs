using BanSach.Models;
using System.Data.Entity;

namespace BanSach.DesignPatterns.CommandPattern
{
    public class DeleteAdminCommand : ICommand
    {
        private readonly int _adminId; // ID của admin cần xóa
        private readonly db_Book _db;  // DbContext để tương tác với database

        // Constructor nhận adminId và DbContext
        public DeleteAdminCommand(int adminId, db_Book db)
        {
            _adminId = adminId;
            _db = db;
        }

        // Thực thi lệnh xóa admin
        public void Execute()
        {
            var admin = _db.Admin.Find(_adminId); // Tìm admin theo ID
            if (admin != null) // Kiểm tra xem admin có tồn tại không
            {
                _db.Admin.Remove(admin); // Xóa admin khỏi DbSet
                _db.SaveChanges();      // Lưu thay đổi vào database
            }
        }

        // Lấy thông tin admin để hiển thị (dùng trong action GET)
        public Admin GetAdmin()
        {
            return _db.Admin.Find(_adminId); // Trả về đối tượng Admin
        }
    }
}



