# WebAPI

Пример приложения **WebAPI** с минимальным работающим функционалом с использованием пакетов: 
 - `Curiosity` - инфраструктурный код
 - `Swagger` - документация
 - `Identity` - базовая авторизация
 
 
 ### Быстрый старт:
 
 Для корректного запуска необходимо указать в файле `config.yml` 
 **актуальную** строку подключения к базе данных **PostgreSQL**
 
 ```yaml
DbOptions:
  ConnectionString: Server=localhost;Port=5432;Database=curiosity_sample;User Id=postgres;Password=qwerty
``` 
а так же строка подключения для мигратора и пользователь которому будет передано право на таблицу, 
после создания

```yaml
MigrationOptions:
  ConnectionString: Server=localhost;Port=5432;Database=curiosity_sample;User Id=postgres;Password=qwerty
  GrantUser: postgres
```


### Отправка email сообщений
Приложение умеет отправлять *email* сообщения, например для сброса пароля. 
Для отправки необходимо указать **актуальные** креды в файле `config.yml` 

```yaml
SmtpEMailOptions:
  SmtpServer: curiosity.sender.ru
  SmtpLogin: curiosity
  SmtpPassword: xxx
  EMailFrom: sample@curiosity.ru
  ReplyTo: support@curiosity.ru
  SenderName: Curiosity
```
