using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BanSach.Models
{
    public partial class DanhMuc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DanhMuc()
        {
            this.DanhMuc_TheLoai = new HashSet<DanhMuc_TheLoai>();
        }

        public int ID { get; set; }

        [DisplayName("Tên Danh Mục")]
        [StringLength(100, ErrorMessage = "Không được quá 100 kí tự")]
        public string TenDanhMuc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhMuc_TheLoai> DanhMuc_TheLoai { get; set; }
    }
}