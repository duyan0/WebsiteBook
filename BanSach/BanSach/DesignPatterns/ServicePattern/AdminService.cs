using BanSach.DesignPatterns.RepositoryPattern;
using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.ServicePattern
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public void CreateAdmin(Admin admin)
        {
            _adminRepository.Add(admin);
            _adminRepository.SaveChanges();
        }
    }
}