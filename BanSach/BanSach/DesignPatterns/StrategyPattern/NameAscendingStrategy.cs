using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StrategyPattern
{
    public class NameAscendingStrategy : ISortStrategy
    {
        public IQueryable<SanPham> Sort(IQueryable<SanPham> products)
        {
            return products.OrderBy(x => x.TenSP);
        }
    }
}