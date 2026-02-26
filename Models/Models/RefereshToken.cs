using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    [Owned]
   public class RefereshToken
    {
        public string refereshToken { get; set; }
        public DateTime exbireon { get; set; }
       
        public DateTime CreatedOn { get; set; }
       
        public DateTime? RevokedOn { get; set; }
        public bool IsExbire => DateTime.UtcNow >= exbireon;
        public bool IActive => RevokedOn == null && !IsExbire;
    }
}
