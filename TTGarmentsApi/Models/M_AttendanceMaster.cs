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
    
    public partial class M_AttendanceMaster
    {
        public decimal Aid { get; set; }
        public decimal ExecutiveId { get; set; }
        public Nullable<System.DateTime> AttendanceDt { get; set; }
        public string AttendanceTime { get; set; }
        public System.DateTime RecTimeStamp { get; set; }
        public string Location { get; set; }
        public string RouteMap { get; set; }
        public string IsDSR { get; set; }
        public Nullable<System.DateTime> IsDSROn { get; set; }
    }
}
