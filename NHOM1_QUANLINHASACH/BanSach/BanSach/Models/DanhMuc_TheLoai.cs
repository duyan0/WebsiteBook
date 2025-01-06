using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public partial class DanhMuc_TheLoai
    {
        public int ID { get; set; }

        [DisplayName("ID Danh Mục")]
        [Required(ErrorMessage = "Danh mục ID không được để trống.")]
        public int DanhMuc_ID { get; set; }

        [DisplayName("ID Thể Loại")]
        [Required(ErrorMessage = "Thể loại ID không được để trống.")]
        public int TheLoai_ID { get; set; }

        [DisplayName("Hình Ảnh")]
        [Required(ErrorMessage = "Hình ảnh không được để trống.")]
        [StringLength(250, ErrorMessage = "Hình ảnh không được vượt quá 250 ký tự.")]
        public string HinhAnh { get; set; }

        public virtual DanhMuc DanhMuc { get; set; }
        public virtual TheLoai TheLoai { get; set; }
    }
}
