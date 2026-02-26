using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Models
{
   public class Owner
    {
        [Key]
        public int OwnerId { get; set; }
        [Required]
        [MaxLength(50)]
        public String FullName { get; set; }
        [Required]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [JsonIgnore]
        public List<House> houses { get; set; } = new();
        public User? user { get; set; }

    }
}
