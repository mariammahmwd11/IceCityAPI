using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Filters
{
   public class OwnerFilter
    {
        public  int pagenumber { get; set; }
        public  int pageSize { get; set; }
        public  string searchName { get; set; }
       
    }
}
