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
    
    public partial class TrnOrderDetail
    {
        public decimal ID { get; set; }
        public decimal OrderNo { get; set; }
        public decimal RetailerID { get; set; }
        public decimal ProductID { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal NetAmount { get; set; }
        public System.DateTime RecTimeStamp { get; set; }
        public Nullable<decimal> DispatchNo { get; set; }
        public string DispatchStatus { get; set; }
        public string color { get; set; }
        public string Size { get; set; }
        public string pcode { get; set; }
        public string pname { get; set; }
        public string HostIp { get; set; }
    }
}
