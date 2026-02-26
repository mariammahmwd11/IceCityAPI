using Business_Layer.DTO;
using Business_Layer.Helper;
using Business_Layer.IServices;
using Data_Access.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Helper;
using Models.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Services
{
    public class AuthService : IAuthService
    {
       
        private readonly IPasswordService passwordService;
        private readonly ApplicationDbContext context;
        private readonly JWTHelper jWT;

        public AuthService(IPasswordService passwordService,ApplicationDbContext context,IOptions<JWTHelper> jWT)
        {
            
            this.passwordService = passwordService;
            this.context = context;
            this.jWT = jWT.Value;
        }
        public JwtSecurityToken GenereteToken(User user)
        {
            var _claims = new List<Claim>();
            _claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            _claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            _claims.Add(new Claim(ClaimTypes.Email, user.Email));
            _claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
            _claims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));
            var symmetricSecurity = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWT.Key));
            var signingCreadintal = new SigningCredentials(symmetricSecurity, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:jWT.Issuer,
                audience:jWT.Audience,
                expires:DateTime.Now.AddMinutes(jWT.ExbirationInMinutes),
                claims: _claims,
                signingCredentials:signingCreadintal



                );
            return token;
        }

        public AuthModel login(LoginDTO loginDTO)
        {
            AuthModel authModel = new();
            var user = context.Users.FirstOrDefault(m => m.Email == loginDTO.Email);
            if(user==null)
            {
                authModel.Message = "There is no user Match this email";
                return authModel;
            }
            var result = passwordService.VarifyHashedPassword(loginDTO.Password, user.PasswordHash);
            if(result==false)
            {
                authModel.Message = "Email or password is invalid";
                return authModel;
            }
            var token = GenereteToken(user);
            if (user.RefreshTokens.Any(b => b.IActive))
            {
                var refreshtocken = user.RefreshTokens.FirstOrDefault(b => b.IActive);
                authModel.RefereshToken = refreshtocken.refereshToken;
                authModel.RefereshTokenExbirationAt = refreshtocken.exbireon;
            }
            var newRefreshToken = GenereteRefershToken();

            if (user.RefreshTokens == null)
                user.RefreshTokens = new List<RefereshToken>();

            user.RefreshTokens.Add(newRefreshToken);

            context.SaveChanges();

            authModel.RefereshToken = newRefreshToken.refereshToken;
            authModel.RefereshTokenExbirationAt = newRefreshToken.exbireon;
            authModel.Message = "Login Successfully";
            authModel.UserId = user.UserId.ToString();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            authModel.TokenExbireAt = DateTime.UtcNow.AddMinutes(jWT.ExbirationInMinutes);
            authModel.Role = user.Role.ToString();
            return authModel;



        }

        public AuthModel Register(RegisterDTO registerDTO)
        {
            var authmodel = new AuthModel();
            var _user =  context.Users.FirstOrDefault(b=>b.Email==registerDTO.Email);
            if(_user!=null)
            {
                authmodel.Message = "This email is token before";
                return authmodel;
            }
            Owner owner=null;
            if(registerDTO.Role==null)
            {
                owner = context.owners.FirstOrDefault(m=>m.Email==registerDTO.Email);
                if(owner==null)
                {
                    authmodel.Message = "There is No Owner Match this email";
                    return authmodel;
                }
            }
            
           
            var DbUser = new User();
            DbUser.UserName = registerDTO.UserName;
            DbUser.PasswordHash = passwordService.HashPassword(registerDTO.Password);
            DbUser.Email = registerDTO.Email;
            DbUser.Role = registerDTO.Role??UserRole.User;
            DbUser.CreatedAt = DateTime.UtcNow;
            DbUser.ownerId = owner?.OwnerId;
            var result = context.Users.Add(DbUser);
            //create token
            var token = GenereteToken(DbUser);
            var refereshtoken = GenereteRefershToken();
            DbUser.RefreshTokens = new List<RefereshToken>();
            DbUser.RefreshTokens.Add(refereshtoken);
            context.SaveChanges();
            
            //assign response
            authmodel.Message = "Registered Successfully";
            authmodel.UserId = DbUser.UserId.ToString();
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            authmodel.TokenExbireAt =DateTime.UtcNow.AddMinutes(jWT.ExbirationInMinutes);
            authmodel.Role = DbUser.Role.ToString();
            authmodel.RefereshToken = refereshtoken.refereshToken;
            authmodel.RefereshTokenExbirationAt = refereshtoken.exbireon;
            return authmodel;
            
        }

       public RefereshToken GenereteRefershToken()
        {
            var num = new byte[32];
            var generetor = RandomNumberGenerator.Create();
            generetor.GetBytes(num);
            return new RefereshToken
            {
                refereshToken = Convert.ToBase64String(num),
                exbireon=DateTime.UtcNow.AddDays(15),
                CreatedOn=DateTime.UtcNow,
                RevokedOn=null
               

            };
        }
        public async Task<AuthModel> RefreshToken(string refreshToken)
        {
            var model = new AuthModel();

            var user = await context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u =>
                    u.RefreshTokens.Any(t => t.refereshToken == refreshToken));

            if (user == null)
            {
                model.Message = "Token is invalid";
                return model;
            }

            var oldToken = user.RefreshTokens
                .Single(t => t.refereshToken == refreshToken);

            if (!oldToken.IActive)
            {
                model.Message = "Token is not active";
                return model;
            }

            var newRefreshToken = GenereteRefershToken();
            user.RefreshTokens.Add(newRefreshToken);

            oldToken.RevokedOn = DateTime.UtcNow;

            var securityToken = GenereteToken(user);

            await context.SaveChangesAsync();

            model.RefereshToken = newRefreshToken.refereshToken;
            model.RefereshTokenExbirationAt = newRefreshToken.exbireon;
            model.Token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            model.TokenExbireAt = DateTime.UtcNow.AddMinutes(jWT.ExbirationInMinutes); 
            model.Role = user.Role.ToString();
            model.UserId = user.UserId.ToString();
            model.Message = "Token refreshed successfully";

            return model;
        }
    }
}
