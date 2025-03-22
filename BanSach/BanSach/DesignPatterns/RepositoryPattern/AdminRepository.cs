using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.RepositoryPattern
{
    public class AdminRepository : IAdminRepository
    {
        private readonly db_Book db;

        public AdminRepository(db_Book context)
        {
            db = context;
        }

        public void Add(Admin admin)
        {
            db.Admin.Add(admin);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public Admin GetAdminById(int id)
        {
            return db.Admin.Find(id);
        }
    }
}