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
    
    public partial class R_FestivePointMaster
    {
        public string Id { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<int> FromPoint { get; set; }
        public Nullable<int> ToPoint { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsRegistration { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}
