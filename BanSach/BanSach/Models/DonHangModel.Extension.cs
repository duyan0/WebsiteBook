// DonHang.Extension.cs
using BanSach.DesignPatterns.StatePattern;
using System;
using System.Linq;

namespace BanSach.Models
{
    public partial class DonHang
    {
        public void InitializeState()
        {
            switch (TrangThai)
            {
                case "Chờ xử lý":
                    State = new PendingState();
                    break;
                case "Đã xác nhận":
                    State = new ConfirmedState();
                    break;
                case "Đã nhận hàng":
                    State = new DeliveredState();
                    break;
                case "Đã huỷ":
                    State = new CancelledState();
                    break;
                case "Yêu cầu trả hàng":
                    State = new ReturnRequestedState();
                    break;
                default:
                    State = new PendingState();
                    break;
            }
        }

        public decimal GetTongSoTien()
        {
            return DonHangCT != null
                ? DonHangCT.Sum(ct => (ct.SoLuong * ct.Gia)) : 0;
        }

        public decimal Total_DH
        {
            get
            {
                return DonHangCT != null
                    ? DonHangCT.Sum(ct => (ct.SoLuong * ct.Gia)) : 0;
            }
        }
    }
}
