using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using BanSach.Models;

namespace BanSach.Controllers
{
    public class VNPAYClient
    {
        private string vnp_TmnCode = "Q6T6EDU4"; // Mã đối tác
        private string vnp_HashSecret = "YYLEU3HSNCK0I7DZMSSR8Q6865ODR2ZB"; // Mã bảo mật
        private string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // URL thanh toán VNPAY

        // Phương thức tạo yêu cầu thanh toán
        public string CreatePaymentRequest(DonHang order)
        {
            var vnp_Params = new Dictionary<string, string>
            {
                { "vnp_Version", "2.0.0" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_TransactionNo", order.IDdh.ToString() }, // ID đơn hàng
                { "vnp_OrderInfo", "Thanh toán đơn hàng #" + order.IDdh },
                { "vnp_OrderType", "BillPayment" },
                { "vnp_Amount", (order.TongTien * 100).ToString() }, // Đảm bảo số tiền là đồng
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html" }, // Thay bằng URL thực tế của bạn
                { "vnp_TxnRef", order.IDdh.ToString() }, // Mã tham chiếu
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
            };

            // Tạo chuỗi dữ liệu cần băm
            string hashData = GetHashData(vnp_Params);

            // Tạo mã bảo mật (SHA256)
            string vnp_SecureHash = GetSecureHash(hashData);

            // Thêm mã bảo mật vào tham số
            vnp_Params.Add("vnp_SecureHash", vnp_SecureHash);

            // Xây dựng chuỗi query từ các tham số
            string query = BuildQuery(vnp_Params);
            string paymentUrl = vnp_Url + "?" + query;

            return paymentUrl;
        }

        // Tạo chuỗi dữ liệu để băm
        public string GetHashData(Dictionary<string, string> vnp_Params)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var param in vnp_Params)
            {
                sb.Append(param.Key + "=" + param.Value + "&");
            }
            return sb.ToString().TrimEnd('&');
        }

        // Tạo mã bảo mật (SHA256)
        public static string GetSecureHash(string hashData)
        {
            byte[] data = Encoding.UTF8.GetBytes(hashData);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(data);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        // Xây dựng chuỗi query từ tham số
        private string BuildQuery(Dictionary<string, string> vnp_Params)
        {
            StringBuilder query = new StringBuilder();
            foreach (var param in vnp_Params)
            {
                query.Append(param.Key + "=" + param.Value + "&");
            }
            return query.ToString().TrimEnd('&');
        }
    }
}
