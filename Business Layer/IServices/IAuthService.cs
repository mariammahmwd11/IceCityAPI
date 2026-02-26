using Business_Layer.DTO;
using Models.Helper;
using Models.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.IServices
{
   public interface IAuthService
    {//هتعامل ان السيستم دا معمول للعملا بتوعي الل ليهم بيانات عندي 
        // فلما باجي اريجستر كيوزر بتشيك ع الايميلز بتاعة الاونر الل عندي 
        AuthModel login(LoginDTO loginDTO);
        AuthModel Register(RegisterDTO registerDTO);
        JwtSecurityToken GenereteToken(User user);
        RefereshToken GenereteRefershToken();
        public  Task<AuthModel> RefreshToken(string refreshToken);


    }
}
