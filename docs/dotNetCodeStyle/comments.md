# Комментарии

Ко всем публичным классам должны быть написаны комментарии. 

Обязательны к комментированию:

- сами классы/enum/делегаты
- публичные методы
- публичные свойства

## Общие требования

### Комментарии пишутся на английском или русском языках

> Зависит от проекта. Но в проекте все должно быть на одном языке.

### В конце комментария должна быть точка

> __Мотивация__
> 
> Соответствуем рекомендациям от Microsoft.

### Комментарии должны быть xml, а не простыми комментариями на C#

> __Неправильно__
    
```csharp
// key for account activation
public string ActivationKey { get; set; }
```

> __Правильно__
    
```csharp
/// <summary>
/// Key for account activation.
/// </summary>
public string ActivationKey { get; set; }
```

### У методов и конструкторов также должны быть комментарии к:

- аргументам
- возвращаемым значениям
- типам, если класс/метод - `generic`

### При наследовании или реализации интерфейсов следует наследовать документацию с помощью `inheritdoc`:

```csharp
/// <inheritdoc />
public class YandexPaymentOptions : IYandexPaymentOptions
{
  /// <inheritdoc />
  public long YandexShopId { get; set; }
}
```

### Комментарии методов должны быть в 3-ем лице

> __Неправильно__
   
```csharp
/// <summary>
/// Проверка опциий и возврат списка ошибок.
/// </summary>
/// <returns>
/// Список ошибок. Если опции валидны, список будет пустым.
/// </returns>
public IReadOnlyCollection<ConfigValidationError> Validate(string prefix = null);
```

> __Правильно__
    
```csharp
/// <summary>
/// Проверяет валидность опциий и возвращает список ошибок.
/// </summary>
/// <returns>
/// Список ошибок. Если опции валидны, список будет пустым.
/// </returns>
public IReadOnlyCollection<ConfigValidationError> Validate(string prefix = null);
```


## Полезные ссылки

- [Официальные рекомендации по комментариям от Microsoft](https://docs.microsoft.com/en-us/dotnet/csharp/codedoc)