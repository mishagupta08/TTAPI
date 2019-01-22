using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TTGarmentsApi.Models
{
    public class ProfilePhoto
    {
        public string memberId { get; set; }
        public string fileBytes { get; set; }
        public string fileName { get; set; }
        public string mediaType { get; set; }
    }
}