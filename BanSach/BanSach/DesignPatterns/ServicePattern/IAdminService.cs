using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.ServicePattern
{
    public interface IAdminService
    {
        void CreateAdmin(Admin admin);
    }
}