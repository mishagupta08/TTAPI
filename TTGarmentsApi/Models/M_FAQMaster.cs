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
    
    public partial class M_FAQMaster
    {
        public decimal ID { get; set; }
        public decimal ExecutiveID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string ActiveStatus { get; set; }
        public System.DateTime RecTimeStamp { get; set; }
    }
}