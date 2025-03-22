using BanSach.Models;
using System.Data.Entity;

namespace BanSach.DesignPatterns.CommandPattern
{
    public class DeleteAdminCommand : ICommand
    {
        private readonly int _adminId;
        private readonly db_Book _db;

        public DeleteAdminCommand(int adminId, db_Book db)
        {
            _adminId = adminId;
            _db = db;
        }

        public void Execute()
        {
            var admin = _db.Admin.Find(_adminId);
            if (admin != null)
            {
                _db.Admin.Remove(admin);
                _db.SaveChanges();
            }
        }

        // Trả về Admin để hiển thị trong action GET
        public Admin GetAdmin()
        {
            return _db.Admin.Find(_adminId);
        }
    }
}