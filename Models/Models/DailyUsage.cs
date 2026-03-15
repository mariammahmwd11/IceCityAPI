using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
  public class DailyUsage
    {
        public int DailyUsageId { get; set; }
        public int HouseId { get; set; }
        public int HeaterId { get; set; }
        public DateTime UsageDate { get; set; }
        public int HoursWorked { get; set; }
        public int HeaterValue { get; set; }
    }
}
