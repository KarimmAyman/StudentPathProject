using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using StudentPath.BLL.Dtos.Accounts;
using StudentPath.BLL.Services.AccountService;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure Database
        builder.Services.AddDbContext<StudentPathContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
        });

        // Configure Identity
        builder.Services.AddIdentity<User, CustomRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 5;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<StudentPathContext>()
        .AddDefaultTokenProviders();

        // Configure JWT Authentication
        //builder.Services.AddAuthentication(options =>
        //{
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        //}).AddJwtBearer(async options =>
        //{
            //    // JWT Security Key
            //    var secretKeyString = builder.Configuration.GetSection("SecretKey").Value;
            //    var secretKeyBytes = Encoding.ASCII.GetBytes(secretKeyString);
            //    var securityKey = new SymmetricSecurityKey(secretKeyBytes);

            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        IssuerSigningKey = securityKey,
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //        ValidAudience = builder.Configuration["Jwt:Audience"],
            //        ValidateLifetime = true,
            //    };
            //});

            // Register Services
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

        // Add JWT Authentication
        builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(Option =>
            {
                #region SecurityKey
                var SecretKeyString = builder.Configuration.GetSection("SecretKey").Value;
                var SecretKeyByte = Encoding.ASCII.GetBytes(SecretKeyString);
                SecurityKey securityKey = new SymmetricSecurityKey(SecretKeyByte);
                #endregion
                Option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                };
            });




            var app = builder.Build();
            // Call the SeedRoles method
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<CustomRole>>();
                await SeedRolesDtocs.SeedRoles(roleManager);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }


    }


