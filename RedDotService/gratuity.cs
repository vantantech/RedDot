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
    
    public partial class gratuity
    {
        public int id { get; set; }
        public int salesid { get; set; }
        public int employeeid { get; set; }
        public decimal amount { get; set; }
    
        public virtual sale sale { get; set; }
    }
}
