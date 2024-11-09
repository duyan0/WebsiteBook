using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;

    public partial class SanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SanPham()
        {
            this.DonHangCT = new HashSet<DonHangCT>();
        }

        public int IDsp { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string TenSP { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm không được để trống")]
        [StringLength(1000, ErrorMessage = "Mô tả sản phẩm không được vượt quá 1000 ký tự")]
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Thể loại không được để trống")]
        public int TheLoai { get; set; }

        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
        public decimal GiaBan { get; set; }

        [Display(Name = "Giá tùy chỉnh")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá tùy chỉnh phải lớn hơn hoặc bằng 0")]
        public decimal? GiaTuTuyChinh { get; set; } // Để lưu giá tùy chỉnh

        [Required(ErrorMessage = "Hình ảnh không được để trống")]
        [StringLength(255, ErrorMessage = "Tên hình ảnh không được vượt quá 255 ký tự")]
        [RegularExpression(@"(.*\.(jpg|jpeg|png|gif)$)", ErrorMessage = "Hình ảnh phải có định dạng .jpg, .jpeg, .png, hoặc .gif")]
        public string HinhAnh { get; set; }


        [Required(ErrorMessage = "ID tác giả không được để trống")]
        public int IDtg { get; set; }

        [Required(ErrorMessage = "ID nhà xuất bản không được để trống")]
        public int IDnxb { get; set; }

        public int IDkm { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không thể nhỏ hơn 0")]
        public int SoLuong { get; set; }

        [Required(ErrorMessage = "Trạng thái sách không được để trống")]
        [StringLength(50, ErrorMessage = "Trạng thái sách không được vượt quá 50 ký tự")]
        public string TrangThaiSach { get; set; }

        public virtual DanhMuc DanhMuc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHangCT> DonHangCT { get; set; }

        public virtual KhuyenMai KhuyenMai { get; set; }
        public virtual NhaXuatBan NhaXuatBan { get; set; }
        public virtual TacGia TacGia { get; set; }
    }
}
