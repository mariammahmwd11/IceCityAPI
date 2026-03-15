
using Business_Layer.IServices;
using Business_Layer.Services;
using Data_Access.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Helper;
using Models.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Week1_Advanced_API_Migrations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllersWithViews();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.Configure<JWTHelper>(builder.Configuration.GetSection("JWT"));
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter JWT Token like this: Bearer {your token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });



            builder.Services.AddDbContext<ApplicationDbContext>(
      option => option.UseSqlServer(builder.Configuration.GetConnectionString("Defult")));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.RequireHttpsMetadata = false;

                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],

                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])
                    ),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["AccessTokenInCookie"];

                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddScoped<IHouseService, HouseService>();
                  builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
                  builder.Services.AddScoped<IPasswordService, PasswordService>();
                  builder.Services.AddScoped<IAuthService, AuthService>();
                  builder.Services.AddScoped<IDashboardService, DashboardService>();
                  builder.Services.AddScoped<IReportService, ReportService>();

                  var app = builder.Build();

                  // Configure the HTTP request pipeline.
                  if (app.Environment.IsDevelopment())
                  {
                      app.UseSwagger();
                      app.UseSwaggerUI();
                  }
            app.UseStaticFiles();
            app.UseAuthentication();
                  app.UseAuthorization();

                  app.MapControllerRoute(
                    name: "areas",
                   pattern: "{area:exists}/{controller=Test}/{action=Index}/{id?}"
              );
                  app.MapControllers();

                  app.Run();
              }
    } 
    }
