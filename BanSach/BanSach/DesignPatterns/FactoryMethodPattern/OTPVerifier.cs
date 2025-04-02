using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace BanSach.DesignPatterns.FactoryMethodPattern
{
    public class OTPVerifier : IVerifier
    {
        public string GenerateVerification()
        {
            return "2111";
        }

        public void SendVerification(string email, string verification)
        {
            var fromAddress = new MailAddress("crandi21112004@gmail.com", "Võ Duy Ân - HUFLIT");
            var toAddress = new MailAddress(email);
            string fromPassword = "wkdo vwwt ufkk kmgh"; // App Password của Gmail

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Xác nhận tài khoản",
                Body = $"Mã OTP của bạn: {verification}"
            })
            {
                smtp.Send(message); // Gửi email chứa OTP
            }
        }
    }
}