using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public partial class TacGia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TacGia()
        {
            this.SanPham = new HashSet<SanPham>();
        }

        public int IDtg { get; set; }

        [DisplayName("Tên Tác Giả")]
        [Required(ErrorMessage = "Tên tác giả không được để trống.")]
        public string TenTG { get; set; }

        [DisplayName("Ngày Sinh")]
        public Nullable<System.DateTime> NgaySinh { get; set; }

        [DisplayName("Quốc Gia")]
        [Required(ErrorMessage = "Quốc gia không được để trống.")]
        public string QuocGia { get; set; }

        [DisplayName("Tiểu Sử")]
        public string TieuSu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPham { get; set; }
    }
}