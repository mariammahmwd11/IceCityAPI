using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.DTO
{
    public class OwnerVm
    {
        public int ownerid { get; set; }
        public String FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int houseCount { get; set; }
        public List<House>? houses { get; set; }
    }
}
