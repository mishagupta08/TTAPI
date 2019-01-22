using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    public class BarcodeDetail
    {
        public string Barcode { get; set; }

        public string RetailerId { get; set; }

        public string LocationX { get; set; }

        public string LocationY { get; set; }
    }
}