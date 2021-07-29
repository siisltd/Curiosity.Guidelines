# Создание HTML шаблонов на основе Razor с использованием Razor.Templating.Core   

## Применения
- Email шаблоны

## Установка Nuget пакета
Данная библиотека доступна как [Nuget пакет](https://www.nuget.org/packages/Razor.Templating.Core/)

##### Установка помощью .NET CLI
```bash
dotnet add package Razor.Templating.Core
```
## Простой пример использования:
```csharp
var confirmAccountModel = new ConfirmAccountEmailViewModel($"{baseUrl}/{Guid.NewGuid()}");

string body = await RazorTemplateEngine.RenderAsync("/Views/Emails/ConfirmAccount/ConfirmAccountEmail.cshtml", confirmAccountModel);

var toAddresses = new List<string> { email };

SendEmail(toAddresses, "donotreply@example.com", "Confirm your Account", body);
```
![Демонстрация Email шаблона](https://github.com/siisltd/Curiosity.Guidelines/blob/razorExample/docs/images/email_template_example.jpg)

##### Для тестирования был использован локальный SMTP клиент [Papercut](https://github.com/ChangemakerStudios/Papercut-SMTP)

