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
    
    public partial class R_TempPointsLedger
    {
        public string Id { get; set; }
        public Nullable<decimal> BarcodeSno { get; set; }
        public string RetailerId { get; set; }
        public Nullable<System.DateTime> EarnSpentDate { get; set; }
        public string LocationX { get; set; }
        public string LocationY { get; set; }
        public string ProductId { get; set; }
        public Nullable<decimal> DabitPoints { get; set; }
        public Nullable<decimal> CreditPoints { get; set; }
        public string Barcode { get; set; }
        public Nullable<int> ProductQty { get; set; }
        public string FirmName { get; set; }
        public string ProductName { get; set; }
    }
}