using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using JwtCoreDemo.Managers;
using Microsoft.IdentityModel.Tokens;
using static JwtCoreDemo.Managers.TokenManager;
using System.Text;

namespace JwtCoreDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenManager, TokenManager>();
            services.AddSingleton<IReaderRepo, ReaderRepo>();

            // Custom extension method added that contains the actual logic
            services.AddBearerAuthentication();
            services.AddMvc();

            services.AddControllers();

            services.AddAuthorization(config =>
            {
                //config.AddPolicy("ShouldBeAReader", options =>
                //{
                //    options.RequireAuthenticatedUser();
                //    options.AuthenticationSchemes.Add(
                //            JwtBearerDefaults.AuthenticationScheme);
                //    options.Requirements.Add(new ShouldBeAReaderRequirement());
                //});

                // Add a new Policy with requirement to check for Admin
                config.AddPolicy("ShouldBeAnAdmin", options =>
                {
                    options.RequireAuthenticatedUser();
                    options.AuthenticationSchemes.Add(
                            JwtBearerDefaults.AuthenticationScheme);
                    options.Requirements.Add(new ShouldBeAnAdminRequirement());
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        } }
        // The Extension class for ServiceCollections
        static class AuthorizationExtension
        {
            // Extension method for Adding 
            // JwtBearer Middleware to the Pipeline
            public static IServiceCollection AddBearerAuthentication(
                this IServiceCollection services)
            {
                var validationParams = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(TokenConstants.key)),
                    ValidIssuer = TokenConstants.Issuer,
                    ValidAudience = TokenConstants.Audience
                };

                var events = new JwtBearerEvents()
                {
                    // invoked when the token validation fails
                    OnAuthenticationFailed = (context) =>
                    {
                        Console.WriteLine(context.Exception);
                        return Task.CompletedTask;
                    },

                    // invoked when a request is received
                    OnMessageReceived = (context) =>
                    {
                        return Task.CompletedTask;
                    },

                    // invoked when token is validated
                    OnTokenValidated = (context) =>
                    {
                        return Task.CompletedTask;
                    }
                };

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme
                        = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme
                        = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = validationParams;
                    options.Events = events;
                });

                return services;
            }
        }

    }
