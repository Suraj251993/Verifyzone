using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NPOI.OpenXmlFormats.Wordprocessing;
using OrgCheck.Middleware;
using System;

namespace OrgCheck
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.  
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Home/Index";
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.SlidingExpiration = true;
                options.ReturnUrlParameter = String.Empty;
            });
            services.AddSession(options =>
            {
                options.Cookie.Name = ".Verifyzone.Session";
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.Preload = true;
                options.MaxAge = TimeSpan.FromHours(1500);
            });
            services.AddHttpContextAccessor();
            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

            services.AddControllersWithViews();
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.Expiration = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });
            string conn = BuildConnectionString(Configuration);
            services.AddDbContext<Models.PostgresContext>(options => options.UseNpgsql(conn));
            // Persisted to the database (not the container's local disk) so tokens/cookies minted before a
            // redeploy still decrypt afterward - containers like Render's get a fresh ephemeral filesystem
            // (and therefore a fresh, incompatible key ring) on every deploy otherwise.
            services.AddDbContext<Models.DataProtectionKeyContext>(options => options.UseNpgsql(conn));
            services.AddDataProtection().PersistKeysToDbContext<Models.DataProtectionKeyContext>();
            var constants = Configuration.GetSection("ApplicationSettings").Get<OrgCheck.Services.Constants>();
            services.AddSingleton(constants);
            services.AddCors(options =>
            {
                options.AddPolicy("RestrictedPolicy", policy =>
                    policy.WithOrigins("https://app.verifyzone.in")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });
            services.AddServices();
            services.AddAutoMapper(typeof(OrgCheckMapperConfiguration));
            services.AddScoped<ExecutionContext, ExecutionContext>();
        }

        // Render (and most container hosts) provide the database as a "postgres://user:pass@host:port/db" URI
        // via DATABASE_URL rather than the ADO-style keyword string Npgsql expects locally. Convert it when present.
        private static string BuildConnectionString(IConfiguration configuration)
        {
            string databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (string.IsNullOrEmpty(databaseUrl))
                return configuration.GetConnectionString("OrgCheckDbConnectionString");

            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':', 2);
            int port = uri.Port > 0 ? uri.Port : 5432;
            return $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    app.UseHsts();
            //}
            //app.UseHsts();  // Disable in development. Enable only during Production build            
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            // Render's edge proxy IP isn't fixed/known in advance, so the default "trusted proxies" allowlist
            // (loopback only) would silently ignore its X-Forwarded-Proto header and cause an HTTPS redirect loop.
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeadersOptions);
            app.Use(async (context, next) =>
            {
                var nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                context.Items["CSPNonce"] = nonce;
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' https://cdn.jsdelivr.net 'nonce-" + nonce + "'; " +
                    "style-src 'self' https://fonts.gstatic.com https://fonts.googleapis.com https://cdnjs.cloudflare.com https://cdn.jsdelivr.net 'nonce-" + nonce + "'; " +
                    "style-src-attr 'unsafe-inline'; " +
                    "img-src 'self' data:; " +
                    "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
                    "connect-src 'self' http://localhost:* ws://localhost:*; " +
                    "frame-ancestors 'none'; " +
                    "object-src 'none';");

                await next();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseRouting();
            app.UseMiddleware<ExecutionContextMiddleware>();
            app.UseAuthorization();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.Strict,
                Secure = CookieSecurePolicy.SameAsRequest
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseCors("RestrictedPolicy");
        }
    }
}
