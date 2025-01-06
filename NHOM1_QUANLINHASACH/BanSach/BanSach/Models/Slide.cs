using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public partial class Slide
    {
        public int Slide_ID { get; set; }

        [DisplayName("Hình Ảnh")]
        [Required(ErrorMessage = "Hình ảnh không được để trống.")]
        [StringLength(250, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 250 ký tự.")]
        public string HinhAnh { get; set; }

        [DisplayName("Mô Tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string MoTa { get; set; }

        [DisplayName("Liên Kết")]
        [StringLength(250, ErrorMessage = "Đường dẫn liên kết không được vượt quá 250 ký tự.")]
        public string Link { get; set; }

        [DisplayName("Thứ Tự")]
        [Required(ErrorMessage = "Thứ tự không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải là số dương.")]
        public Nullable<int> ThuTu { get; set; }
    }
}
