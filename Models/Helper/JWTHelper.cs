using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Helper
{
   public class JWTHelper
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public double ExbirationInMinutes { get; set; }
        public string Key { get; set; }
    }
}
