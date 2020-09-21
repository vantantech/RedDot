//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebAccess.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class sale
    {
        public sale()
        {
            this.gratuities = new HashSet<gratuity>();
            this.salesitems = new HashSet<salesitem>();
            this.payments = new HashSet<payment>();
        }
    
        public int id { get; set; }
        public int userid { get; set; }
        public int ticketno { get; set; }
        public Nullable<int> customerid { get; set; }
        public Nullable<System.DateTime> saledate { get; set; }
        public Nullable<System.DateTime> lastupdated { get; set; }
        public Nullable<decimal> adjustment { get; set; }
        public string status { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> subtotal { get; set; }
        public string note { get; set; }
        public Nullable<int> employeeid { get; set; }
        public string custom1 { get; set; }
        public string custom2 { get; set; }
        public string custom3 { get; set; }
        public string custom4 { get; set; }
        public Nullable<int> rewardexception { get; set; }
        public Nullable<int> stationno { get; set; }
    
        public virtual ICollection<gratuity> gratuities { get; set; }
        public virtual ICollection<salesitem> salesitems { get; set; }
        public virtual ICollection<payment> payments { get; set; }
    }
}