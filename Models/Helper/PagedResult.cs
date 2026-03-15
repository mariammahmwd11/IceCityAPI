using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Helper
{
   public class PagedResult<T> where T:class
    {
        public  int  PageNumber { get; set; }
        public  int  PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages =>(int) Math.Ceiling((decimal)(TotalItems / PageSize));
        public List<T> Data { get; set; }

    }

}
