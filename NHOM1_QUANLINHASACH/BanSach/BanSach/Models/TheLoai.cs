using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public partial class TheLoai
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TheLoai()
        {
            this.DanhMuc_TheLoai = new HashSet<DanhMuc_TheLoai>();
            this.SanPham = new HashSet<SanPham>();
        }

        public int ID { get; set; }

        [DisplayName("Tên Thể Loại")]
        [Required(ErrorMessage = "Tên thể loại không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên thể loại không được vượt quá 200 ký tự.")]
        public string TenTheLoai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhMuc_TheLoai> DanhMuc_TheLoai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPham { get; set; }
    }
}
