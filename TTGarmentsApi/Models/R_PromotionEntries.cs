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
    
    public partial class R_PromotionEntries
    {
        public string Id { get; set; }
        public string PromotionId { get; set; }
        public string RetailerId { get; set; }
        public string ImageUrl { get; set; }
        public Nullable<System.DateTime> UpoadedDate { get; set; }
        public Nullable<bool> IsValid { get; set; }
    }
}
