using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public partial class Banner
    {
        public int Banner_ID { get; set; }

        [DisplayName("Hình Ảnh")]
        [Required(ErrorMessage = "Hình ảnh không được để trống.")]
        public string HinhAnh { get; set; }

        [DisplayName("Mô Tả")]
        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [StringLength(250, ErrorMessage = "Mô tả không được vượt quá 250 ký tự.")]
        public string MoTa { get; set; }

        [DisplayName("Liên Kết")]
        [Required(ErrorMessage = "Liên kết không được để trống.")]
        public string Link { get; set; }

        [DisplayName("Thứ Tự")]
        [Required(ErrorMessage = "Thứ tự không được để trống.")]
        public Nullable<int> ThuTu { get; set; }
    }
}
