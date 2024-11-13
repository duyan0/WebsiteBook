namespace BanSach.Models
{
    using System.Collections.Generic;

    public partial class KhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhachHang()
        {
            this.DonHang = new HashSet<DonHang>();
        }

        public int IDkh { get; set; }

        public string TenKH { get; set; }

        public string SoDT { get; set; }


        public string Email { get; set; }


        public string TKhoan { get; set; }
        public string MKhau { get; set; }
        public string ConfirmPass { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHang> DonHang { get; set; }
    }
}
