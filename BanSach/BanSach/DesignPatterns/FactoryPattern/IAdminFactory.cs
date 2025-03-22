using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.FactoryPattern
{
    public interface IAdminFactory
    {
        db_Book CreateDbContext();
    }
}