using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}