using GCCars.Application.Data;
using GCCars.Application.Extensions;
using GCCars.Application.Settings;
using GCCars.Domain.Models.Identity;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using GCCars.Application.Hubs;
using GCCars.Application.Middleware;
using Microsoft.AspNetCore.SignalR;
using GCCars.Api.Filters;
using System.Threading.Tasks;

namespace GCCars.Api
{
    public class Startup
    {
        private const string DEFAULT_CONNECTION = "DefaultConnection";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("Cors", builder =>
            {
                builder.SetIsOriginAllowed(h => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));
            services.Configure<AdminUserSettings>(Configuration.GetSection(nameof(AdminUserSettings)));
            services.Configure<AuthSettings>(Configuration.GetSection(nameof(AuthSettings)));

            services.AddDbContext<AppDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString(DEFAULT_CONNECTION),
                opt => opt.MigrationsAssembly(typeof(DbContext).Assembly.ToString())));

            services.AddIdentity<AppUser, AppRole>(o =>
            {
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<AppDbContext>();

            var authSettings = Configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new()
                    {
                        ValidateAudience = true,
                        ValidAudience = authSettings.Audience,
                        ValidateIssuer = true,
                        ValidIssuer = authSettings.Issuer,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = authSettings.GetSymmetricSecurityKey()
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/platform") || path.StartsWithSegments("/pvp")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddAuthorization(o => o.DefaultPolicy = new AuthorizationPolicy(new[] { new DenyAnonymousAuthorizationRequirement() }, new[] { JwtBearerDefaults.AuthenticationScheme }));

            services.AddHangfire(o =>
            {
                o.UseSqlServerStorage(Configuration.GetConnectionString(DEFAULT_CONNECTION));
            });
            services.AddHangfireServer();

            services.AddApplicationServices();
            services.AddValidators();
            services.AddMappingProfiles();

            services.AddHealthChecks();

            services.AddScoped<SignalRErrorFilter>();
            services.AddSignalR(o =>
            {
                o.AddFilter<SignalRErrorFilter>();
            })
                .AddJsonProtocol(o => o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
            services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GCCars.Api", Version = "v1" });
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Please insert token into the field.",
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RpCoin v1");
                c.DefaultModelsExpandDepth(-1);
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("Cors");
            app.UseHealthChecks("/health");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/queues");

            app.UseGlobalErrorHandler();
            ApplyMigrations(context);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PvpBattlesHub>("/pvp");
                endpoints.MapHub<PlatformHub>("/platform");
                endpoints.MapControllers();
            });
        }
        
        private void ApplyMigrations(AppDbContext context)
        {
            try
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}
