using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.FactoryMethodPattern
{
    public interface IVerifier
    {
        string GenerateVerification(); // Tạo mã xác nhận
        void SendVerification(string email, string verification); // Gửi mã xác nhận qua email
    }
}