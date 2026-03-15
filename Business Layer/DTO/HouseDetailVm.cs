using Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.DTO
{
   public class HouseDetailVm
    {
        public int HouseId { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string CityZone { get; set; }
        public List<DailyUsage> DailyUsages { get; set; }
        public double todayUsage { get; set; }
        public List<Heater> heaters { get; set; }

    }
}
