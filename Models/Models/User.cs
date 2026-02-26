using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer.Helper;

namespace Models.Models
{
   public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        //هعوز اربط اليوزر بالاونر ف حالة ان الرول عندي يوزر بحيث انه لما يلوجين يكون متاح ليه يشوف الريبورتس الخاصه ببيته مثلا 
        public int? ownerId { get; set; }
        public Owner? owner { get; set; }
        public ICollection<RefereshToken> RefreshTokens { get; set; }

    }
}
