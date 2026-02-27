using Business_Layer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Helper
{
  public  class AuthModel
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public DateTime TokenExbireAt { get; set; }
        public bool IsAuthenticated { get; set; }
        //refereshtoken
        public string RefereshToken { get; set; }
        public DateTime RefereshTokenExbirationAt { get; set; }

    }
}
