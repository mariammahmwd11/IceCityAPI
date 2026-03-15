using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.DTO
{
    public class DashboardOverviewVm
    {
        public int TotalOwners { get; set; }
        public int TotalHouses { get; set; }
        public double TotalConsumption { get; set; }
        public double TodayConsumption { get; set; }
        public List<HouseDetailVm> top5Houses { get; set; }
    }
}
