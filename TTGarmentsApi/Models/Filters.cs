using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    public class Filters
    {
        public int From { get; set; }

        public int To { get; set; }

        public int FromPoint { get; set; }

        public int ToPoint { get; set; }

        /**Points ledger filter**/
        public string FilterValue { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string SelectedFilterName { get; set; }
    }
}