using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.DTO
{
  public  class ReportOptions
    {
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public bool pdf { get; set; } = false;
        public bool csv { get; set; } = false;


    }
}
