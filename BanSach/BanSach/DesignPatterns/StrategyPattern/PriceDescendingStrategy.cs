using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.StrategyPattern
{
    public class PriceDescendingStrategy : ISortStrategy
    {
        public IQueryable<SanPham> Sort(IQueryable<SanPham> products)
        {
            return products.OrderByDescending(x =>
                x.KhuyenMai != null && x.KhuyenMai.MucGiamGia > 0
                ? x.GiaBan * (1 - (decimal)x.KhuyenMai.MucGiamGia / 100)
                : x.GiaBan);
        }
    }
}