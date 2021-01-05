using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShowIssueTracker.Models;
using ShowIssueTracker.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using ShowIssueTracker.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShowIssueTracker.Data;

namespace ShowIssueTracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                                options.UseInMemoryDatabase("ShowIssueTracker"));


            services.AddSingleton<IFileProvider>(
           new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));


            services.AddSingleton(Configuration);
            services.Configure<SendGridProperties>(Configuration.GetSection("SendGridSettings"));
            services.Configure<InternalProperties>(Configuration.GetSection("Internal"));



            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

            })
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //Setting cookies lifetime. 
            //This configen has only something to say if isPersient is set to true in ExternalLogin.cshtml.cs, as: await _signInManager.SignInAsync(user, isPersistent: true)
            //Or 'Remember me' is set, when using AspNet Identity login Login.cshtml.cs (await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberLogin, lockoutOnFailure: true);)
            services.ConfigureApplicationCookie(options =>
            {
                var COOKIE_TIMESPAN = 120;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(COOKIE_TIMESPAN);
                options.Cookie.IsEssential = true;
                options.SlidingExpiration = true;
            });

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");

            services.Configure<GlobalConstants>(Configuration.GetSection("GlobalConstants"));
            services.Configure<ImpexiumProperties>(Configuration.GetSection("ImpexiumProperties"));
            services.Configure<SendGridProperties>(Configuration.GetSection("SendGridSettings"));
            services.Configure<JWToken>(Configuration.GetSection("Tokens"));

            //services.AddIdentityServer()
            // .AddDeveloperSigningCredential()
            // .AddInMemoryPersistedGrants()
            // .AddInMemoryIdentityResources(Config.GetIdentityResources())
            // .AddInMemoryApiResources(Config.GetApiResources())
            // .AddInMemoryClients(Config.GetClients())
            // .AddAspNetIdentity<ApplicationUser>();

            services.AddAuthentication(IdentityConstants.ApplicationScheme)
               //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(jwtoptions =>
               {
                   jwtoptions.Audience = Configuration["Tokens:Audience"];
                   jwtoptions.ClaimsIssuer = Configuration["Tokens:Issuer"];
                   //AutomaticAuthenticate = true,
                   //AutomaticChallenge = true,
                   jwtoptions.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidIssuer = Configuration["Tokens:Issuer"],
                       ValidAudience = Configuration["Tokens:Audience"],
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))

                   };
               });

            services.AddScoped<IAuthorizationHandler, ClaimsHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.Requirements.Add(new ClaimsRequirement("Admin"));
                });
                options.AddPolicy("View", policy =>
                {
                    policy.Requirements.Add(new ClaimsRequirement("View"));
                });
                options.AddPolicy("Edit", policy =>
                {
                    policy.Requirements.Add(new ClaimsRequirement("Edit"));
                });
            });


            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
                //.AllowCredentials());
            });

            services.AddRazorPages()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                .AddMvcOptions(options => options.EnableEndpointRouting = false);

            // Add application services. 
            services.AddScoped<IGlobalExceptionLoggingMiddleWare, GlobalExceptionLoggingMiddleWare>();
            services.AddScoped<IImpexiumLogin, ImpexiumLogin>();
            services.AddScoped<INadaRepository, NadaRepository>();
           // services.AddSingleton<IDocumentDBRepository<Item>>(new DocumentDBRepository<Item>());

            services.AddScoped<IDocumentDBRepository<Item>, DocumentDBRepository<Item>>();

            services.AddRazorPages();
            services.AddMvc().AddNewtonsoftJson();
            services.AddMvc().AddRazorRuntimeCompilation();
            services.AddControllers().AddNewtonsoftJson();
            services.AddControllers();
            services.AddControllersWithViews();

            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
          //  app.UseIdentityServer();
            app.UseAuthorization();
            app.UseStatusCodePages();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default", "{controller=Item}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
    public class GlobalConstants
    {
        public string SuperUserEmail { get; set; }
        public string SuperUserPassword { get; set; }
    }
}

