//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RedDotService
{
    using System;
    using System.Collections.Generic;
    
    public partial class store
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public store()
        {
            this.AspNetUsers = new HashSet<AspNetUser>();
        }
    
        public int id { get; set; }
        public string storename { get; set; }
        public Nullable<int> WebUserId { get; set; }
        public string storecode { get; set; }
        public string connectionstring { get; set; }
        public string hmac { get; set; }
        public string salt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
