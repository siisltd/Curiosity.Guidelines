using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Curiosity.Samples.WebApp.API.Configuration;
using Curiosity.Samples.WebApp.API.Middleware;
using Curiosity.Tools.IoC;
using Curiosity.Tools.Web.Middleware;
using Curiosity.Tools.Web.ReverseProxy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace Curiosity.Samples.WebApp.API.Startup
{
    public class Startup
    {
        private const string CorsPolicyName = "SampleCorsPolicy";

        private readonly IConfiguration _rawConfig;
        private AppConfiguration? _typedConfig;
        private AppConfiguration Configuration => _typedConfig ??= _rawConfig.Get<AppConfiguration>();

        public Startup(IConfiguration rawConfig)
        {
            _rawConfig = rawConfig ?? throw new ArgumentNullException(nameof(rawConfig));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Configure<WebEncoderOptions>(options =>
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All)
            );

            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

            services.AddControllers(options =>
                {
                    // фильтры запросов
                    options.Filters.Add(typeof(ValidationModelFilter));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson();

            services.AddRouting(options => options.LowercaseUrls = true);

            // авторизация
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration.AuthOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = Configuration.AuthOptions.Audience,

                        IssuerSigningKey = Configuration.AuthOptions.SymmetricSecurityKey,
                        ValidateIssuerSigningKey = true,

                        ValidateLifetime = true,
                    };
                });
            services.AddAuthorization();

            // CORS policy
            services.AddCors(o => o.AddPolicy(CorsPolicyName, builder =>
            {
                builder.WithOrigins(
                        Configuration.Urls,
                        Configuration.ExternalUrls.WebSite,
                        "http://localhost:8080") // URL с которого локальный фронт может обращаться к dev api
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            // имена полей в JSON данные в которых будут скрываться в логах
            services.AddSensitiveDataProtector(new[]
            {
                "Password",
                "PasswordConfirmation",
                "CurrentPassword",
            });

            // генерим документацию
            services.AddAppSwagger();

            // идентити
            services.AddAppIdentity(Configuration);

            // добавим свои сервисы
            services.AddWebAppServices(Configuration);

            // зарегистрируем сервисы чтоб использовать в кодовых миграциях
            services.AddSingleton(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // ловим исключения
            app.UseMiddleware<ExceptionFilterMiddleware>();

            // нужно для работы за реверс прокси (но и без него на работу не влияет)
            // должен идти до всяких проверок авторизации
            app.UseReverseProxy(Configuration.ReverseProxy);

            app.UseStaticFiles();
            app.UseRouting();

            // CORS должен идти после роутинга, но перед авторизацией
            app.UseCors(CorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();

            // документация
            app.UseAppSwagger(Configuration);

            if (Configuration.LogRequestContent)
            {
                app.UseMiddleware<RequestLoggingMiddleware>();
            }

            app.UseEndpoints(endpoints => { endpoints.MapControllerRoute("default", "{controller=home}/{action=index}"); });
        }
    }
}
