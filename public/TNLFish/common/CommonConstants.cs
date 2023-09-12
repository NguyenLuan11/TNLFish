using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNLFish.Models;

namespace TNLFish.common
{
    // Chứa những biến cố định
    public static class CommonConstants
    {
        public static ShopCaCanhEntities db = new ShopCaCanhEntities(); 

        public static string USER_SESSION = "USER_SESSION";
        public static string CurrentCulture { get; set; }
    }
}