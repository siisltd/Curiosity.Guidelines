# Исключения

Для обработки исключений используется промежуточный слой `Middleware.ExceptionFilterMiddleware.cs`

> Важно размещать обработчик исключений первым среди остальных слоев.
 
 ```c#
public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionFilterMiddleware>();
        
        // other layers
    }
}
```

## Как это работает

- Ожидаемые исключения (наследованные от `AppException`) записываются в `WARN` лог, 
    преобразуются в модель `Response` со списком ошибок

- Остальные исключения записываются в `ERROR` лог 
    (NLog отправляет уведомление разработчикам, что произошло неожиданное исключение)
    на фронт отпрвляется стандартный `Response` о критической ошибке на сервере
 
 - Все интересующие данные о запросе (path, query, body, token) 
    извлекаются и записываются в лог
    
## SensitiveDataProtector

Что бы исключить потенциальную утечку паролей пользователей из логов добавляем 
    `Curiosity.Tools.SensitiveDataProtector`, который скрывает данные заданных полей.
    
```c#
_dataProtector.HideInJson(body)
```
    
Для этого необходимо в `Startup` добавить список полей данные в которых будут скрываться

```c#
services.AddSensitiveDataProtector(new[]
{
    "Password",
    "PasswordConfirmation",
    "CurrentPassword",
});
```
