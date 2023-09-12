using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNLFish.Models
{
    public class MailModel
    {
        public string From { get; set; }

        public string To = "tnlfish20422@gmail.com";

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}