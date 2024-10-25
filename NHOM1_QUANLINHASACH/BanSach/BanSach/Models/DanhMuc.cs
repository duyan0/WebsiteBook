using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public partial class DanhMuc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DanhMuc()
        {
            this.SanPham = new HashSet<SanPham>();
        }

        public int ID { get; set; }

        // Ràng buộc không được để trống và độ dài tối đa là 50 ký tự
        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự.")]
        
        public string DanhMuc1 { get; set; }

        // Ràng buộc không được để trống và độ dài tối đa là 50 ký tự
        [Required(ErrorMessage = "Thể loại là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Thể loại không được vượt quá 50 ký tự.")]
        
        public string TheLoai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPham { get; set; }
    }
}
