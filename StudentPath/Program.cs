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
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
        });
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
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
 .AddJwtBearer(options =>
 {
     var secretKeyString = builder.Configuration["SecretKey"];
     if (string.IsNullOrEmpty(secretKeyString))
     {
         throw new Exception("JWT Secret Key is missing in configuration.");
     }

     var secretKeyBytes = Encoding.ASCII.GetBytes(secretKeyString);
     var securityKey = new SymmetricSecurityKey(secretKeyBytes);

     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = securityKey,

         ValidateIssuer = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],

         ValidateAudience = true,
         ValidAudience = builder.Configuration["Jwt:Audience"],

         ValidateLifetime = true, // Ensure token expiry is checked
         ClockSkew = TimeSpan.Zero // Removes default 5 min leeway for token expiration
     };
 });


        builder.Services.AddMemoryCache();



        var app = builder.Build();
            // Call the SeedRoles method
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<CustomRole>>();
                await SeedRolesDtocs.SeedRoles(roleManager);
            }

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }


    }


