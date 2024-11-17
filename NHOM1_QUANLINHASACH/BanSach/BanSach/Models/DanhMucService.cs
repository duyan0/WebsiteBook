using System;
using System.Collections.Generic;
using System.Linq;

namespace BanSach.Models
{


    public class DanhMucService
    {
        private readonly dbSach db;
        private readonly DanhMucFactory danhMucFactory;

        public DanhMucService(dbSach context, DanhMucFactory factory)
        {
            db = context;
            danhMucFactory = factory;
        }

        public List<string> GetDanhMucList()
        {
            return db.DanhMuc.Select(d => d.DanhMuc1).ToList();
        }

        public bool IsDanhMucExists(string danhMuc1, string theLoai)
        {
            return db.DanhMuc.Any(d => d.DanhMuc1 == danhMuc1 && d.TheLoai == theLoai);
        }

        public void AddDanhMuc(int id, string danhMuc1, string theLoai)
        {
            var danhMuc = danhMucFactory.CreateDanhMuc(id, danhMuc1, theLoai);
            db.DanhMuc.Add(danhMuc);
            db.SaveChanges();
        }
    }
}
