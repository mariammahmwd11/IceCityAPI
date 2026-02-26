using Business_Layer.IServices;
using Microsoft.AspNetCore.Identity;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHasher<User> hasher;
        public PasswordService(IPasswordHasher<User> hasher)
        {
            this.hasher = hasher;
        }

        public string HashPassword(string PassWord)
        {
            var hashedPassword = hasher.HashPassword(null,PassWord);
            return hashedPassword;
        }

        public bool VarifyHashedPassword(string Password, string HashedPassword)
        {
            var result=hasher.VerifyHashedPassword(null, HashedPassword, Password);

            return result == PasswordVerificationResult.Success;
        }
    }
}
