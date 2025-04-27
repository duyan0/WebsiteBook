using BanSach.DesignPatterns.StatePattern;
using BanSach.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Globalization;

public class PendingState : IOrderState
{
    public void UpdateStatus(DonHang donHang, db_Book db)
    {
        // Chuyển sang trạng thái "Đã xác nhận"
        donHang.TrangThai = "Đã xác nhận";
        donHang.State = new ConfirmedState();
        db.Entry(donHang).State = EntityState.Modified;
        db.SaveChanges();

        // Gửi email thông báo
        SendOrderNotificationEmail(donHang);
    }

    public string GetStatus() => "Chờ xử lý";

    private void SendOrderNotificationEmail(DonHang donHang)
    {
        string customerEmail = donHang.KhachHang.Email;
        string subject = $"Đơn hàng {donHang.IDdh} của bạn đã được xác nhận";

        // Đọc nội dung template HTML
        string templatePath = HttpContext.Current.Server.MapPath("~/Templates/Emails/OrderConfirmationTemplate.html");
        string body = File.ReadAllText(templatePath);

        // Thay thế các placeholder bằng dữ liệu thực tế
        body = body.Replace("{TenKH}", donHang.KhachHang.TenKH)
                   .Replace("{IDdh}", donHang.IDdh.ToString())
                   .Replace("{NgayDatHang}", donHang.NgayDatHang?.ToString("dd/MM/yyyy HH:mm:ss"))
                   .Replace("{TrangThai}", donHang.TrangThai)
                   .Replace("{Total_DH}", donHang.Total_DH.ToString("N0", new CultureInfo("vi-VN")) + " ₫")
                   .Replace("{DiaChi}", donHang.DiaChi);

        // Cấu hình gửi email
        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new System.Net.NetworkCredential("crandi21112004@gmail.com", "wxaq spxp ghcr nyji"),
            Timeout = 30000
        };

        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress("crandi21112004@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(customerEmail);

        try
        {
            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
        }
    }
}