using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.DTO
{
    public class DailyUsageVm
    {
        public DateTime Date { get; set; }
        public int HoursWorked { get; set; }
        public decimal HeaterValue { get; set; }
    }
}
