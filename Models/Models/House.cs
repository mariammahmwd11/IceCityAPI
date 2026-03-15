using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Models
{
  public  class House
    {
        [Key]
        public int HouseId { get; set; }
        [ForeignKey("owner")]
        public int OwnerId { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string CityZone { get; set; }
        public Owner owner { get; set; }
        [JsonIgnore]
        public List<Heater> heaters { get; set; } = new();
        public List<DailyUsage> DailyUsages { get; set; }

    }
}
