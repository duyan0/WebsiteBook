using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BanSach.Models
{
    public partial class NhaXuatBan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhaXuatBan()
        {
            this.SanPham = new HashSet<SanPham>();
        }

        public int IDnxb { get; set; }

        [DisplayName("Tên Nhà Xuất Bản")]
        [Required(ErrorMessage = "Tên nhà xuất bản không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên nhà xuất bản không được vượt quá 200 ký tự.")]
        public string Tennxb { get; set; }

        [DisplayName("Địa Chỉ")]
        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự.")]
        public string DiaChi { get; set; }

        [DisplayName("Số Điện Thoại")]
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        public string SDT { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự.")]
        public string Email { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPham { get; set; }
    }
}
