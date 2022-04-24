using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using Tasks4U.Data;
using Tasks4U.Models;
using Tasks4U.Services;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Tasks4U
{
    public class Startup
    {
        public IConfiguration Config { get; }

        public Startup(IConfiguration config)
        {
            Config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Tasks4UDbContext>(options =>
                options.UseSqlServer(Config.GetConnectionString(nameof(Tasks4UDbContext))));

            services
                .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<Tasks4UDbContext>();

            services
                .AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/TodoItems/List", "{handler=get}/{pageNum?}");
                })
                .AddRazorRuntimeCompilation();

            services
                .AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Config.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });

            services.AddAutoMapper(typeof(Startup));

            services.AddHostedService<MigratorService>();

            services.AddControllersWithViews();
            services.AddHttpClient("emailClient", client => client.Timeout = TimeSpan.FromMinutes(5))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddScoped<IEmailService, EmailService>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });
            services.AddApplicationInsightsTelemetry(Config["APPINSIGHTS_CONNECTIONSTRING"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCookiePolicy();
            app.UseStaticFiles(
                new StaticFileOptions()
                {
                    OnPrepareResponse = (context => context.Context.Response.Headers.Add(HeaderNames.CacheControl, "public, max-age=30"))
                });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages().RequireAuthorization();
            });
        }

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                //Samesite.None Cookies are disabled

                //var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                // TODO: Use your User Agent library of choice here. 
                //if (userAgent.Contains("Edg/86"))
                //{
                options.SameSite = SameSiteMode.Unspecified;
                //}
            }
        }
    }
}
