using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    using DocumentFormat.OpenXml.Wordprocessing;
    using System;
    using System.Collections.Generic;

    public partial class DanhMuc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DanhMuc()
        {
            this.SanPham = new HashSet<SanPham>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự")]
        public string DanhMuc1 { get; set; }

        [Required(ErrorMessage = "Thể loại không được để trống")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Thể loại không được vượt quá 50 ký tự")]

        public string TheLoai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPham { get; set; }
    }
}