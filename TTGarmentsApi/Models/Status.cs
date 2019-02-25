using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    public enum Status
    {
        Pending = 0, 
        Confirm = 1,
        Delivered = 2,
        Rejected = 3,
        OnHold = 4
    }
}