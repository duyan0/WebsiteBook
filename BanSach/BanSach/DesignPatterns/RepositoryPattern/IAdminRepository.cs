using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.RepositoryPattern
{
    public interface IAdminRepository
    {
        void Add(Admin admin);
        void SaveChanges();
        Admin GetAdminById(int id);
    }
}