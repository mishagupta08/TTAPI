using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    public class DashboardCounts
    {
        public int RetailerCount { get; set; }

        public decimal TotalEarnPoint { get; set; }

        public decimal TotalRedeemPoint { get; set; }

        public int SannedBarcode5Count { get; set; }

        public int SannedBarcode10Count { get; set; }

        public int SannedBarcode15Count { get; set; }

        public int SannedBarcodeCount { get; set; }

        public int RejectOrderCount { get; set; }

        public int PendingOrderCount { get; set; }

        public int ConfirmCount { get; set; }

        public int DeliveredOrderCount { get; set; }

        public int OnHoldCount { get; set; }
    }
}