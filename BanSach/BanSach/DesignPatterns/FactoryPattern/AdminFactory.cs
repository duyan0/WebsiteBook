using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.FactoryPattern
{
    public class AdminFactory : IAdminFactory
    {
        public db_Book CreateDbContext()
        {
            return new db_Book(); // Tạo mới DbContext
        }
    }
}