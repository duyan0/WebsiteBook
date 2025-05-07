using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public partial class SanPham
    {
        public SanPham Clone()
        {
            return new SanPham()
            {
                IDsp = this.IDsp,
                TenSP = this.TenSP,
                MoTa = this.MoTa,
                IDtl = this.IDtl,
                GiaBan = this.GiaBan,
                HinhAnh = this.HinhAnh,
                IDtg = this.IDtg,
                IDnxb = this.IDnxb,
                IDkm = this.IDkm,
                SoLuong = this.SoLuong,
                TrangThaiSach = this.TrangThaiSach,
                NgayPhatHanh = this.NgayPhatHanh,
                ISBN = this.ISBN,
                SoTrang = this.SoTrang,
                NgonNgu = this.NgonNgu,
                LuotXem = this.LuotXem,
                KichThuoc = this.KichThuoc,
                TrongLuong = this.TrongLuong,
                NgayTao = this.NgayTao,
                NgayCapNhat = this.NgayCapNhat,
                DiemDanhGiaTrungBinh = this.DiemDanhGiaTrungBinh
            };
        }
    }
}