using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RelatedWordsAPI.App
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int TokenExpiryInDays { get; set; }
        public string HttpUserAgent { get; set; }
    }
}
