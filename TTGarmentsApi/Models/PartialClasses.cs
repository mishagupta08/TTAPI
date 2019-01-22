using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    public partial class R_RetailerMaster
    {
        //public string StateName { get; set; }

        //public string CityName { get; set; }

        //public decimal Points { get; set; }

        //public double Distance { get; set; }

        public string DateString { get; set; }
    }

    public partial class R_PointsLedger
    {
        //public string FirmName { get; set; }
        public string EarnSpentDateString { get; set; }
        //public string ProductName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Nullable<decimal> BalancePoint { get; set; }

    }

    public partial class R_FestivePointMaster
    {
        public string DateString { get; set; }
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
    }

        public partial class R_UploadedMedia
    {
        public string CategoryName { get; set; }
        public string DateString { get; set; }
    }

    public partial class R_NotificationManager
    {
        public string DateString { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Exception { get; set; }

        public decimal? FailMessageCount { get; set; }
        public decimal? SuccessMessageCount { get; set; }
        public decimal? NotificationIdCount { get; set; }
        public string ResultDate { get; set; }
    }

    public partial class R_AppVersion
    {
        public string DateString { get; set; }
    }

    public partial class R_MessageMaster
    {
        public string DateString { get; set; }
        public string PublishFromDate { get; set; }
        public string PublishToDate { get; set; }
    }

    public partial class R_NotificationReply
    {
        public string RetailerName { get; set; }

        public string DateString { get; set; }
    }

    public partial class R_BannerMaster
    {
        public string DateString { get; set; }
    }

    public partial class R_OrderMaster
    {
        public string Retailer { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string DateString { get; set; }
    }

    public partial class R_Promotion
    {
        public int RetailerStatus { get; set; }
    }

    public class Result
    {
        public string message_id { get; set; }
        public string error { get; set; }
    }

    public class NotificationResponse
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public List<Result> results { get; set; }
    }

}