using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    public class OrderRequest
    {
        public string RetailerId { get; set; }

        public string LocationX { get; set; }

        public string LocationY { get; set; }

        public IList<ProductOrderDetail> ProductDetail = new List<ProductOrderDetail>();

    }
}