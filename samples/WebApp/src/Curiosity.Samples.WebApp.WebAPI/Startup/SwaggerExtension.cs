using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Curiosity.Samples.WebApp.API.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Curiosity.Samples.WebApp.API.Startup
{
    public static class SwaggerExtension
    {
        private const string ApiTitle = "Curiosity sample API";

        /// <summary>
        /// Добавляем сваггер с нашими настройками 
        /// </summary>
        public static IServiceCollection AddAppSwagger(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<ReadOnlySchemaFilter>();

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = ApiTitle,
                    Version = $"v{Assembly.GetExecutingAssembly().GetName().Version}",
                    Description = ApiDescriptionBuilder.BuildDescription(),
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // авторизация в swagger UI
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
                {
                    Name = JwtBearerDefaults.AuthenticationScheme,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    },
                    In = ParameterLocation.Header
                };
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {key, new List<string>()}
                });
            });

            return services;
        }

        public static IApplicationBuilder UseAppSwagger(this IApplicationBuilder app, AppConfiguration configuration)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    var host = httpReq.Host.Value;
                    var scheme = (configuration.Urls ?? "").Contains(host) ? "http" : "https"; // костыль, потому что тут https нормально не определяется, хз почему так
                    swagger.Servers = new List<OpenApiServer> {new OpenApiServer {Url = $"{scheme}://{host}"}};
                });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiTitle);
                c.DocumentTitle = ApiTitle;
            });

            return app;
        }
    }

    /// <summary>
    /// Фильтр который скрывает из документации POST методов поля помеченные [ReadOnly(true)]
    /// </summary>
    public class ReadOnlySchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
            {
                return;
            }

            foreach (var schemaProperty in schema.Properties)
            {
                var property = context.Type.GetProperty(schemaProperty.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    if (property.GetCustomAttributes(typeof(ReadOnlyAttribute), false).SingleOrDefault() is ReadOnlyAttribute attr && 
                        attr.IsReadOnly)
                    {
                        if (schemaProperty.Value.Reference != null)
                        {
                            schemaProperty.Value.AllOf = new List<OpenApiSchema>
                            {
                                new OpenApiSchema
                                {
                                    Reference = schemaProperty.Value.Reference
                                }
                            };
                            schemaProperty.Value.Reference = null;
                        }

                        schemaProperty.Value.ReadOnly = true;
                    }
                }
            }
        }
    }
}