using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.FactoryMethodPattern
{
    public class VerifierFactory
    {
        public static IVerifier CreateVerifier(string method)
        {
            switch (method.ToLower())
            {
                case "otp":
                    return new OTPVerifier(); // Trả về đối tượng OTPVerifier
                default:
                    throw new ArgumentException("Phương thức xác nhận không được hỗ trợ.");
            }
        }
    }
}