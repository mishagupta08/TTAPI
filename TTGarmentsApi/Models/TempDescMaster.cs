//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TTGarmentsApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TempDescMaster
    {
        public decimal id { get; set; }
        public string Description { get; set; }
        public string ActiveStatus { get; set; }
        public decimal UserID { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> ActionOn { get; set; }
        public Nullable<decimal> ByUserID { get; set; }
    }
}
