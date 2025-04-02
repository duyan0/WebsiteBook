using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.CommandPattern
{
    //Đây là giao diện cơ bản của Command Pattern,
    //định nghĩa một phương thức Execute() mà tất cả các lệnh (command) phải triển khai.
    public interface ICommand
    {
        void Execute();
    }
}