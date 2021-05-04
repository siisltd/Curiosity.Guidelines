# Авторизация

Для авторизации в `WebAPI` используем `JWT`.

> Коротко, что такое `JWT`, можно узнать по [ссылке](https://jwt.io/).

Для авторизации через `JWT` в наше приложение нужно:

- В конфигурацию добавить секцию со следующими полями:
    ```yaml
    # Опции авторизации
    AuthOptions:
        # Имя издателя токена
        Issuer: Curiosity sample API
        # Имя потребителя токена
        Audience: Curiosity sample Client
        # ключ для генерации токена (при смене - все старые токены не валидны)
        Key: 1075F0D5-0D2F-4639-9D18-271AC715B581
        # длинна жизни токена
        LifeDays: 30
    ```
- Устанавить пакет `Microsoft.AspNetCore.Authentication.JwtBearer`

- В классе `Startup` настроить авторизация и аутентификацию как на примере ниже: 
    ```csharp
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
    ```

    ```c#
    public void Configure(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
    ```

Для генерации токена используем `JwtSecurityTokenHandler`. 
Полезную информацию записываем в виде `Claims`.
Используем `System.Security.Claims.ClaimTypes` для имен.

    ```c#
    public string GenerateToken(UserEntity user)
    {
        var now = _dateTimeService.GetCurrentTimeUtc();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FIO),
            new Claim(ClaimTypes.Role, user.Client.Role), 
            new Claim(ClientIdClimeName, user.Client.Id.ToString()),
            new Claim(ClaimTypes.Version, "1"), // версия токена. При добавлении новых claim - версия + 1 (для фронта)
        };

        var jwt = new JwtSecurityToken(
            issuer: _configuration.AuthOptions.Issuer,
            audience: _configuration.AuthOptions.Audience,
            notBefore: now,
            expires: now.AddDays(_configuration.AuthOptions.LifeDays),
            claims: claims,
            signingCredentials: new SigningCredentials(_configuration.AuthOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

        return TokenHandler.WriteToken(jwt);
    }
    ```
Продебажить токен можно на сайте: https://jwt.io

Для успешной авторизации необходимо добавлять токен в хедер запроса. Пример:

    ```yaml
    -H  "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjUiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoi0J_QvtC00YjRg9C8INCf0YDQuNCx0L7QtdCyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWdlbnQiLCJDbGllbnRJZCI6IjMiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ZlcnNpb24iOiIxIiwibmJmIjoxNjE3MjcyMzc1LCJleHAiOjE2MTk4NjQzNzUsImlzcyI6IklURyBCYW5rIEd1YXJhbnRlZXMgQVBJIiwiYXVkIjoiSVRHIEJhbmsgR3VhcmFudGVlcyBDbGllbnQifQ.FalR5clCPYxUzlrVkolGVSNlZoZx-uZceGQl7ryFCac"
    ```

Для управления доступа к методам API используем атрибуты:

- `[AllowAnonymous]` - авторизация не требуется

- `[Authorize]` - авторизация обязательна. 
Для неавторизированных запросов будет приходит ответ с кодом 401

- `[Authorize(Roles = "Admin,Agent")]` - для успешной авторизации 
в Claims должна быть записана роль Admin или Agent 
`"http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Agent"`

- для более сложных кейсов авторизации можно использовать Policy
    
 
Для получения id авторизованного пользователя на беке, так же используем Claims,
которые достаются из токена в слое `Authorization` и помещаются в `HttpContext.User` . Например, так (если есть Identity):

```c#
public long GetUserId(HttpContext httpContext)
{
    var claimValue = _signInManager.UserManager.GetUserId(httpContext.User) ?? "";
    if (!long.TryParse(claimValue, out var userId))
        throw new InvalidOperationException($"Невозможно получить id пользователя из claims (claim value = \"{claimValue}\")");

    return userId;
}
```

Более подробно код см. `BLL.Auth.AuthService.cs` в `sample/WebApp`.