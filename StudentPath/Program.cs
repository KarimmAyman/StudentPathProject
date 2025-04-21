using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using StudentPath.BLL.AutoMappers.DriverMapper;
using StudentPath.BLL.AutoMappers.UserMapper;
using StudentPath.BLL.AutoMappers.TripMapper;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtos.Accounts;
using StudentPath.BLL.Middlewares;
using StudentPath.BLL.Services.AccountService;
using StudentPath.BLL.Services.ActivityService;
using StudentPath.BLL.Services.AdminServices;
using StudentPath.BLL.Services.DriverServices;
using StudentPath.BLL.Services.HousingServices;
using StudentPath.BLL.Services.PaymobService;
using StudentPath.BLL.Services.StripeService;
using StudentPath.BLL.Services.UserServices;
using StudentPath.BLL.Services.TripServices;
using StudentPath.BLL.Utility;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.ActivitesRepository;
using StudentPath.DAL.Repositories.HousingRepository;
using StudentPath.DAL.Repositories.UnitOfWork;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
    
        // Add services to the container.
        builder.Services.AddControllers();

        #region Swagger

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(setup =>
        {
            setup.SchemaFilter<SwaggerIgnoreFilter>();

            setup.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UMSSTHA System - KFS-FCI",
                Version = "v1",
                Description = "API For UMSSTHA System",
                Contact = new OpenApiContact
                {
                    Name = "UMSSTHA System",
                    Email = "umssthasystem@gmail.com"
                }



            });
            setup.EnableAnnotations();

            // Include 'SecurityScheme' to use JWT Authentication
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Put *ONLY* your JWT Bearer token on textbox below!",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });

        });


        #endregion

        #region Connection String

        // Configure Database
        builder.Services.AddDbContext<StudentPathContext>(options =>
        {
            options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("cs"));
        });

        #endregion


        #region Identity

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
        #endregion

     
        #region AutoMapper
        builder.Services.AddAutoMapper(x => x.AddProfile(new UserProfile()));
        builder.Services.AddAutoMapper(x => x.AddProfile(new DriverProfile()));
        builder.Services.AddAutoMapper(x => x.AddProfile(new TripProfile()));




        #endregion


        #region Services
        // Register Services
        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("Stripe:SecretKey");

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy.AllowAnyOrigin()// Your frontend URL
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                );
        });

        builder.Services.AddScoped<IAccountService, StudentPath.BLL.Services.AccountService.AccountService>();
         builder.Services.AddScoped<IEmailService, EmailService>();
         builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
        builder.Services.AddScoped<IPropertyService, PropertyService>();
        builder.Services.AddScoped<IDriverService, DriverService>();
        builder.Services.AddScoped<StripeService>();
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<PaymobService>();
        builder.Services.AddScoped<IJobRepository, JobRepository>();
        builder.Services.AddScoped<IJobService, JobService>();
        builder.Services.AddScoped<IAdminService, AdminService>();
        builder.Services.AddScoped<ITripService, TripService>();



        builder.Services.AddHttpContextAccessor();

        #endregion


        #region JWT

        // Add JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
 .AddJwtBearer(options =>
 {
     var secretKeyString = builder.Configuration["JWT:SecretKey"];
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
         ValidIssuer = builder.Configuration["JWT:Issuer"],

         ValidateAudience = true,
         ValidAudience = builder.Configuration["JWT:Audience"],

         ValidateLifetime = true, // Ensure token expiry is checked
         ClockSkew = TimeSpan.Zero // Removes default 5 min leeway for token expiration
     };
 });

        




        #endregion


        #region MemoryCache
        builder.Services.AddMemoryCache();
        #endregion



        var app = builder.Build();

        #region MiddleWare
        app.UseMiddleware<GlobalExceptionMiddleware>();
        #endregion



        #region Stripe

        #endregion


        #region Seed Roles

        // Call the SeedRoles method
        using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<CustomRole>>();
                await SeedRolesDtocs.SeedRoles(roleManager);
            }
        #endregion
      
        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseSwagger();
        //    app.UseSwaggerUI();
        //}
            app.UseSwagger();
            app.UseSwaggerUI();
        app.UseCors("AllowFrontend");
        app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }


    }


