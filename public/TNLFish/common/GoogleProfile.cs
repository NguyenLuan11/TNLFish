using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNLFish.common
{
    public class GoogleProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ImageProfile Image { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
    }
}