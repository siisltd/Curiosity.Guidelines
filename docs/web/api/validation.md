# Валидация

Все данные, которые приходят в запросах, проходят опциональную валидацию при помощи атрибутов `System.ComponentModel.DataAnnotations`. Swagger хорошо умеет документировать эти атрибуты.

## Как это работает

Свойства в модели запроса помечаются атрибутами 

```c#
public class PasswordRequestModel
{
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
}
```

Так же атрибутами может быть помечен аргумент метода в контроллере:

```c#
public async Task<Response> UploadLicenses([MinLength(1)] IFormFileCollection files) 
```

Чтобы валидация атрибутами заработала, нужно сделать свой `ActionFilter`:

- Реализуем интерфейс `IAsyncActionFilter` (пример `Middleware.ValidationModelFilter.cs`)

```c#
    public class ValidationModelFilter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modelState = context.ModelState;

            // если модель валидна выполняем действие
            if (modelState.IsValid)
            {
                return next();
            }

            // если не валидна - бросаем исключение (обработается в ExceptionFilterMiddleware)
            var errors = new List<InvalidRequestDataError>(modelState.Count);
            foreach (var item in modelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    errors.Add(new InvalidRequestDataError(error.ErrorMessage, item.Key));
                }
            }

            throw new InvalidRequestDataException(errors);
        }
    }
 ```
    
    Если модель не прошла валидацию на уровне МVС, фильтр будет выбрасывать исключение с кодом ошибки, означающем невалидные данные запроса.

- Добавим фильтр в классе `Startup` 
    ```c#
    services.AddControllers(options =>
    {
        options.Filters.Add(typeof(ValidationModelFilter));
    });
    ```

- Обязательно добавляем слой обрабтки ошибок, чтобы наше исключение было залогированно и передано в понятном для фронта виде:
    ```c#
    app.UseMiddleware<ExceptionFilterMiddleware>();
    ```

## Список наиболее часто используемых атрибутов

- `[Required]` - требует обязательное наличие значения для nullable полей

- `[Range(1, 100)]` - задаёт доступный диапазон для числовых типов

- `[MinLength(1)]` - не допускает пустые строки и массивы

- `[MaxLength(200)]` - ограничение для длины строки. Используется, когда на длину строки есть ограничение в БД

- `[RegularExpression(@"^[0-9]+$")]` - проверка строки регулярным варажением

- `[Compare("Password", ErrorMessage = "Пароли не совпадают")]` - сравнивает значение двух полей

- `[ValidEmail(ErrorMessage = "Не корректный e-mail")]` - атрибут `Curiosity` - проверяет валидность email адреса.
    (Стандартный атрибут с задачей справляется плохо)

- `[ReadOnly(true)]` - атрибут, который говорит сваггеру, что данное поле только для чтения и его не нужно показывать в документации POST запроса (сваггер ещё нужно научить это понимать).

- `[JsonConverter(typeof(TrimStringConverter))]` - данным атрибутом помечаются поля в модели `[FromBody]` и при биндинге у строки автоматически обрезаются пробелы (для моделей `[FromQuery]` и `[FromForm]` используется другой механизм биндинга и данный атрибут работать не будет).
 