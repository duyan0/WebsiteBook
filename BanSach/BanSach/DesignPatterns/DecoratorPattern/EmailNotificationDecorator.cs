using BanSach.Controllers;
using BanSach.DesignPatterns.StrategyPattern;
using BanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.DecoratorPattern
{
    public class EmailNotificationDecorator : IOrderStatusStrategy
    {
        private readonly IOrderStatusStrategy _strategy;
        private readonly DonHangsController _controller;
        private readonly int _orderId;

        public EmailNotificationDecorator(IOrderStatusStrategy strategy, DonHangsController controller, int orderId)
        {
            _strategy = strategy;
            _controller = controller;
            _orderId = orderId;
        }

        public void UpdateStatus(DonHang donHang, db_Book db)
        {
            _strategy.UpdateStatus(donHang, db); // Gọi strategy gốc
            _controller.SendOrderNotificationEmail(_orderId); // Gửi email sau khi cập nhật
        }
    }
}