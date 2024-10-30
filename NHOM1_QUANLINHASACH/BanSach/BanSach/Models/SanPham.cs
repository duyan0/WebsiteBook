namespace BanSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class SanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SanPham()
        {
            this.DonHangCT = new HashSet<DonHangCT>();
        }

        public int IDsp { get; set; }

        [Display(Name = "Tên Sách")]
        [Required(ErrorMessage = "Tên sách là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tên sách không được vượt quá 200 ký tự.")]
        public string TenSP { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string MoTa { get; set; }

        [Display(Name = "Thể loại")]
        [Required(ErrorMessage = "Thể loại là bắt buộc.")]
        public int TheLoai { get; set; }

        [Display(Name = "Giá bán")]
        [Required(ErrorMessage = "Giá bán là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0.")]
        [DisplayFormat(DataFormatString = "{0:N0} VND", ApplyFormatInEditMode = true)]
        public decimal GiaBan { get; set; }

        [Display(Name = "Giá tùy chỉnh")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá tùy chỉnh phải lớn hơn hoặc bằng 0.")]
        public decimal? GiaTuTuyChinh { get; set; } // Để lưu giá tùy chỉnh

        [Display(Name = "Hình ảnh")]
        [StringLength(200, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 200 ký tự.")]
        public string HinhAnh { get; set; }

        [Display(Name = "Tác giả")]
        [Required(ErrorMessage = "Tác giả là bắt buộc.")]
        public int IDtg { get; set; }

        [Display(Name = "Nhà xuất bản")]
        [Required(ErrorMessage = "Nhà xuất bản là bắt buộc.")]
        public int IDnxb { get; set; }

        
        public System.DateTime NamXB { get; set; }

        [Display(Name = "Số lượng")]
        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
        public int SoLuong { get; set; }

        [Display(Name = "Trạng thái")]
        [Required(ErrorMessage = "Trạng thái sách là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự.")]
        public string TrangThaiSach { get; set; }

        public virtual DanhMuc DanhMuc { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHangCT> DonHangCT { get; set; }
        public virtual NhaXuatBan NhaXuatBan { get; set; }
        public virtual TacGia TacGia { get; set; }
    }
}
