using BanSach.DesignPatterns.StatePattern;
using BanSach.Models;
using System;
using System.Data.Entity;
using System.Net.Mail;

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
        string body = $@"
        <html>
        <body>
            <h2>Xin chào {donHang.KhachHang.TenKH},</h2>
            <p>Đơn hàng của bạn đã được xác nhận.</p>
            <p><strong>Thông tin đơn hàng:</strong></p>
            <table border='1' cellpadding='5' cellspacing='0'>
                <tr><td><strong>Mã đơn hàng:</strong></td><td>{donHang.IDdh}</td></tr>
                <tr><td><strong>Ngày đặt hàng:</strong></td><td>{donHang.NgayDatHang?.ToString("dd/MM/yyyy HH:mm:ss")}</td></tr>
                <tr><td><strong>Trạng thái:</strong></td><td>{donHang.TrangThai}</td></tr>
                <tr><td><strong>Tổng tiền:</strong></td><td>{donHang.Total_DH.ToString("C")}</td></tr>
                <tr><td><strong>Địa chỉ giao hàng:</strong></td><td>{donHang.DiaChi}</td></tr>
            </table>
            <p>Cảm ơn bạn đã mua hàng tại chúng tôi!</p>
        </body>
        </html>";

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