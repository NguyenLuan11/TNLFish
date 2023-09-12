using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNLFish.common
{
    // Đối tượng UserLogin dùng trong Session
    [Serializable]
    public class UserLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        // Dùng phân quyền
        public string GroupID { get; set; }
    }
}