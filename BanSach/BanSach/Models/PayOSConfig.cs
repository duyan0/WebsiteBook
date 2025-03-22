using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.Models
{
    public class PayOSConfig
    {
        public string ClientId { get; set; } = "your-client-id";
        public string ApiKey { get; set; } = "your-api-key";
        public string ChecksumKey { get; set; } = "your-checksum-key";
    }
}