# Документация

Для документирования и тестирования API используем `Swagger` - штуку для автоматизированного создания документации `WebAPI` на основе стандартной документации C#.

В `Swagger` попадают `xml` комментарии к публичным методам контроллеров, их аргументам и публичные поля моделей. 


Для генерации `xml` документации добавим в файл `.csproj`

```xml
<PropertyGroup>
    <!-- включение генерации xml файла-->
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <!-- Отключаем уведомления компилятора, о том что не для всех свойств добавлены комментарии-->
    <!-- потому что далеко не везде документирующие комменты нужны-->
    <NoWarn>$(NoWarn);1591</NoWarn>
    <NoWarn>$(NoWarn);1573</NoWarn>
</PropertyGroup>
```

## Настройка


- Устанавливаем пакеты 
    - `Swashbuckle.AspNetCore.SwaggerGen`
    - `Swashbuckle.AspNetCore.SwaggerUI`
    
- Настраиваем в SwaggerUI в `Startup`

    ```c#
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            // название страницы SwaggerUI
            Title = "Curiosity sample API",
            // версию API берем из Assembly
            // выводим чтоб лучше детектить обновления
            Version = $"v{Assembly.GetExecutingAssembly().GetName().Version}",
            // любая дополнительная текстовая информация для потребителя API
            Description = ApiDescriptionBuilder.BuildDescription(),
        });

        // берем документацию из xml файла
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });
    ```


- Для тестирования API очень удобно использовать SwaggerUI. 
Ниже учим SwaggerUI использовать авторизацию с JWT токеном:

    ```c#
    services.AddSwaggerGen(c =>
    {
        // авторизация в swagger UI
        c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
        {
            // Bearer
            Name = JwtBearerDefaults.AuthenticationScheme,
            // находится в хедере
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
    ```


- Добавим слой для обслуживания созданного документа JSON и пользовательского интерфейса Swagger:

    ```c#
    public void Configure(IApplicationBuilder app)
    {
        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger(c =>
        {
            // костыль, потому что тут https нормально не определяется
            // это нужно если хотим тестировать апи из Swagger UI
            c.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                var host = httpReq.Host.Value;
                var scheme = (configuration.Urls ?? "").Contains(host) ? "http" : "https";
                swagger.Servers = new List<OpenApiServer> {new OpenApiServer {Url = $"{scheme}://{host}"}};
            });
        });

        // specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Curiosity sample API");
            c.DocumentTitle = "Curiosity sample API";
        });
    }

    ```


## Расширение документации

Чтобы не создавать разные модели для деталей и обновления объекта мы помечаем поля 
только для чтения атрибутом `[ReadOnly(true)]`. 

Потребитель API будет видеть в POST методе только те поля, которые он может обновлять.
Для того чтоб `Swagger` научился скрывать такие поля необходимо ещё в `Startup` добавить фильтр:

    ```c#
    services.AddSwaggerGen(c =>
    {
        c.SchemaFilter<ReadOnlySchemaFilter>();
    });
    ```


    ```c#
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
    ```


Для вывода дополнительной информации (например коды ошибок и числовые значения enums) используем класс `Startup.ApiDescriptionBuilder.cs` из примера.

