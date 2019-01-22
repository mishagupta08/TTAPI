using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    public class NotificationResult
    {
        public string Message { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
    }
}