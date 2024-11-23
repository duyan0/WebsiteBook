using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class ThongKeDonHangViewModel
    {
        // Tổng số lượng đơn hàng
        public int TongSoDonHang { get; set; }

        // Tổng số lượng đơn hàng đang chờ xử lý
        public int DonHangChoXuLy { get; set; }

        // Tổng số lượng đơn hàng đã xác nhận
        public int DonHangDaXacNhan { get; set; }

        // Tổng số lượng đơn hàng đã nhận hàng
        public int DonHangDaNhanHang { get; set; }

        // Tổng số lượng đơn hàng đã huỷ
        public int DonHangDaHuy { get; set; }

        // Tổng doanh thu từ các đơn hàng đã hoàn thành (đã nhận hàng)
        public decimal TongDoanhThu { get; set; }
    }
}
