using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BanSach.Models
{
    public class PayOSService
    {
        private readonly PayOSConfig _config;
        private readonly db_Book _db; // Thêm db_Book

        public PayOSService(db_Book db = null) // Constructor với db_Book tùy chọn
        {
            _config = new PayOSConfig();
            _db = db ?? new db_Book(); // Sử dụng db_Book hiện tại hoặc tạo mới
        }

        // Tạo đơn hàng và lấy URL thanh toán, nhận thêm ID khách hàng
        public async Task<string> CreatePaymentUrl(decimal amount, string orderId, string orderInfo, string returnUrl, string cancelUrl, int customerId)
        {
            if (amount <= 0)
                throw new ArgumentException("Số tiền phải lớn hơn 0.");
            if (string.IsNullOrEmpty(orderId))
                throw new ArgumentException("Mã đơn hàng không được để trống.");
            if (string.IsNullOrEmpty(returnUrl) || string.IsNullOrEmpty(cancelUrl))
                throw new ArgumentException("URL trả về và hủy phải hợp lệ.");

            // Lấy thông tin khách hàng từ database
            var customer = await _db.KhachHang.FindAsync(customerId);
            if (customer == null)
            {
                throw new Exception("Không tìm thấy thông tin khách hàng.");
            }

            var paymentData = new
            {
                amount = (long)(amount * 100), // Số tiền (VNĐ, nhân 100 vì đơn vị nhỏ nhất là xu)
                orderCode = orderId, // Mã đơn hàng duy nhất
                orderDescription = orderInfo, // Mô tả đơn hàng
                buyerName = customer.TenKH, // Tên người mua từ database
                buyerEmail = customer.Email, // Email người mua từ database
                buyerPhone = customer.SoDT, // SĐT người mua từ database
                returnUrl = returnUrl, // URL trả về sau thanh toán
                cancelUrl = cancelUrl, // URL khi hủy thanh toán
                currency = "VND" // Tiền tệ (VNĐ)
            };

            using (var client = new HttpClient())
            {
                
                client.DefaultRequestHeaders.Add("x-client-id", _config.ClientId);
                client.DefaultRequestHeaders.Add("x-api-key", _config.ApiKey);

                // Đặt timeout ngắn hơn (ví dụ: 30 giây)
                client.Timeout = TimeSpan.FromSeconds(30);

                var json = JsonConvert.SerializeObject(paymentData);
                System.Diagnostics.Debug.WriteLine("PayOS Request: " + json); // Log để debug
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync("/payment-requests", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine("PayOS Response: " + responseContent); // Log phản hồi
                        var paymentResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        return paymentResponse.checkoutUrl.ToString(); // URL thanh toán
                    }
                    throw new Exception($"Lỗi PayOS: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
                catch (HttpRequestException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi kết nối PayOS: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                    throw new Exception("Lỗi kết nối PayOS: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi tạo đơn hàng PayOS: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                    throw new Exception("Lỗi tạo đơn hàng PayOS: " + ex.Message, ex);
                }
            }
        }        // Xác minh thanh toán (callback/notify)
        public bool VerifyPayment(string data, string signature)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_config.ChecksumKey));
            var computedSignature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
            return computedSignature == signature;
        }
    }
}