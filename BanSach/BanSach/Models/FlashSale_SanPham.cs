//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FlashSale_SanPham
    {
        public int ID { get; set; }
        public int IDfs { get; set; }
        public int IDsp { get; set; }
    
        public virtual FlashSale FlashSale { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}
