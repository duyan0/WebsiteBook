using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class DanhMucFactory
    {
        public DanhMuc CreateDanhMuc(int id, string danhMuc1, string theLoai)
        {
            return new DanhMuc
            {
                ID = id,
                DanhMuc1 = danhMuc1,
                TheLoai = theLoai
            };
        }
    }
}