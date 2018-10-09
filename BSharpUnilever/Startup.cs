using BSharpUnilever.Data;
using BSharpUnilever.Data.Entities;
using BSharpUnilever.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;

namespace BSharpUnilever
{
    public class Startup
    {
        public IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register identity
            services.AddIdentity<User, IdentityRole>(opt =>
            {
                // Users will be represented by their confirmed emails
                opt.User.RequireUniqueEmail = true;

                // Make the password requirement less annoying, and compensate by increasing required length
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 7;
            })

            // Providers that issue email-confirmation and password-reset tokens
            .AddDefaultTokenProviders()

            // Register BSharpContext as the identity store
            .AddEntityFrameworkStores<BSharpContext>();

            // The code below configures web API with JWT token authentication, further reading: https://jwt.io/introduction/
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // The tokens will be validated against the secret config parameters below
            // The same config parameters are accessible to the bSharp endpoint issuing those token
            // Note: We use a symmetric key since the same app is issuing AND validating the tokens
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = _config["Tokens:Issuer"],
                    ValidAudience = _config["Tokens:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
                };
            });

            // Register our db context, to make it available for dependency injection
            services.AddDbContext<BSharpContext>(opt =>
                opt.UseSqlServer(_config.GetConnectionString("DefaultConnection")));


            // TODO determine if AddAuthorization is necessary


            // Register MVC, the JSON options instruct the serializer to keep property names in PascalCase, 
            // even though this isn't convention, it makes a few things easier since both client and server
            // side sides get to see and communicate identical property names, for example 'api/customers?orderby='Name'
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new DefaultNamingStrategy() })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // Custom services
            services.AddTransient<BSharpContextSeeder>();
            services.AddSingleton<IEmailSender, SendGridEmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BSharpContextSeeder seeder)
        {
            // Create the admin user who is able to create other users 
            seeder.CreateAdminUserAndRolesAsync().Wait();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
