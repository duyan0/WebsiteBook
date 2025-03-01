using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BanSach.Models
{
    public class MomoHelper
    {
        private const string Endpoint = "https://test-payment.momo.vn/v2/gateway/api/create"; // Kiểm tra môi trường
        private const string PartnerCode = "MOMOXXXX"; // Thay bằng PartnerCode thực tế
        private const string AccessKey = "F8BBA842ECF85"; // Thay bằng AccessKey của bạn
        private const string SecretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz"; // Thay bằng SecretKey
        private const string RequestType = "captureWallet";

        public static async Task<string> CreatePaymentRequest(string orderId, long amount, string returnUrl, string notifyUrl)
        {
            var requestId = Guid.NewGuid().ToString();
            var orderInfo = "Thanh toán đơn hàng " + orderId;

            var rawData = $"accessKey={AccessKey}&amount={amount}&extraData=&ipnUrl={notifyUrl}&orderId={orderId}" +
                          $"&orderInfo={orderInfo}&partnerCode={PartnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType={RequestType}";

            var signature = ComputeHmacSha256(rawData, SecretKey);

            var payload = new
            {
                partnerCode = PartnerCode,
                accessKey = AccessKey,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                extraData = "",
                requestType = RequestType,
                signature
            };

            using (var client = new HttpClient())
            {
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Endpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Momo API Response: " + responseString); // Log response để debug

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);

                // Kiểm tra lỗi từ API Momo
                if (jsonResponse == null || jsonResponse.payUrl == null)
                {
                    throw new Exception($"Không nhận được URL thanh toán từ Momo. API Response: {responseString}");
                }

                return jsonResponse.payUrl;
            }
        }

        private static string ComputeHmacSha256(string message, string secretKey)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
