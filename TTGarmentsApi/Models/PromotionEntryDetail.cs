using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    public class PromotionEntryDetail
    {
        public string PromoId { get; set; }

        public string PromoHeading { get; set; }

        public string PromoText { get; set; }

        public string RetailerId { get; set; }

        public string RetailerFirmName { get; set; }

        public int UploadImagecount { get; set; }

        public List<string> ImageUrls { get; set; }

        public Nullable<bool> IsApproved { get; set; }

        public int ApprovedImagecount { get; set; }
    }
}