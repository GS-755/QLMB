//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLMB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web;

    public partial class MatBang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MatBang()
        {
            this.DonXinThues = new HashSet<DonXinThue>();
        }
    
        public string MaMB { get; set; }
        public int Lau { get; set; }
        public double DienTich { get; set; }
        public int Khu { get; set; }
        public double TienThue { get; set; }
        public short MATT { get; set; }
        public string HinhMB { get; set; }
        [NotMapped]
        public HttpPostedFileBase UploadImage { get; set; } 
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonXinThue> DonXinThues { get; set; }
        public virtual TinhTrang TinhTrang { get; set; }
    }
}
