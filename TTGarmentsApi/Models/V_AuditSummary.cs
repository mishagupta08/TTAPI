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
    
    public partial class V_AuditSummary
    {
        public decimal MasterID { get; set; }
        public decimal ExecutiveId { get; set; }
        public decimal AuditID { get; set; }
        public decimal RetailerId { get; set; }
        public string FirmName { get; set; }
        public string ContactPerson { get; set; }
        public string AuditDescription { get; set; }
        public string ImagePath { get; set; }
        public Nullable<System.DateTime> AuditQueDate { get; set; }
        public Nullable<System.DateTime> AuditExpiryDate { get; set; }
        public decimal Points { get; set; }
        public string DisplayStatus { get; set; }
        public Nullable<System.DateTime> DisplayDate { get; set; }
        public string DisplayImagePath { get; set; }
        public string GeoLocation { get; set; }
        public string ConfirmStatus { get; set; }
        public string ConfirmStatusVal { get; set; }
        public Nullable<System.DateTime> AuditConfirmDate { get; set; }
        public string AuditType { get; set; }
        public string ExecutiveNm { get; set; }
        public string UserId { get; set; }
        public string Remarks { get; set; }
    }
}