using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    /// <summary>
    /// Holds response detail
    /// </summary>
    public class Response
    {
        /// <summary>
        /// gets or sets status
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// gets or sets response value
        /// </summary>
        public string ResponseValue { get; set; }

        /// <summary>
        /// gets or sets url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// gets or sets Points
        /// </summary>
        public decimal Points { get; set; }

        /// <summary>
        /// gets or sets TotalRecords
        /// </summary>
        public decimal TotalRecords { get; set; }

        /// <summary>
        /// gets or sets CartProductCount
        /// </summary>
        public int CartProductCount { get; set; }
    }
}