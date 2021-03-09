# Логирование

На проде продебажить не получится, а проблемы возникают, их надо решать. В этом помогают логи. 
Но писать логи нужно тоже грамотно.

На бэкенде мы используем `NLog`.

Язык логов - русский.

## Уровни логирования

Чтобы не засорять лог ненужной информацией, логи надо разделать по уровням.

| Log level  | Когда использовать  | Примеры |
|---|---|---|
| Fatal  | Произошла непоправимая ошибка, приложение лучше остановить  | Не придумал) |
| Error  |  Произошла ошибка, приложение в целом может продолжить работу | Выброшено исключение |
| Warning  | Произошла странная ситуация, но приложение работает нормально  | Данные не прошли валидацию, не найден объект |
| Information  | Запись о нормальном событии. Лучше сюда выводить информацию о выполнении бизнес-задач  | Отправлен Email, добавлен пользователь, загружен файл, выполнено несколько этапов бизнес-задачи
| Debug  | Используем для отладки  | Выполнился запрос, пользователь авторизовался |
| Trace  | Для низкоуровневой трассировки  | Начался выполняться метод X, он закончился |

> Об ошибках нужно сообщать на почту, благо NLog это делает практически из коробки

### NLog

### Вывод в логи `TraceId`

Каждому входящему запросу `Kestrel` присваивает уникальный идентификатор, который позволяет в логах искать записи,
которые относятся к конкретному запросу. Для включения такой опции нужно добавить в `layout` `${aspnet-TraceIdentifier}`
```xml
<target xsi:type="Console"
            name="console"
            layout="${longdate} ${level:upperCase=true} ${aspnet-TraceIdentifier} ${message}${onexception:${newline}${exception:format=tostring}}"
            detectConsoleAvailable="true" />
```

> Это будет работать, если установлен пакет `NLog.Web.AspNetCore`. В других случаях ничего не будет.

### Отправка логов на почту

В `NLog.config` добавить секцию:

```xml
<extensions>
    <add assembly="NLog.MailKit" />
</extensions>
```

В приложение установить `NLog.MailKit`

Добавить `target` и заполнить поля:

```xml
<target name="mailLimitied"
        xsi:type="LimitingWrapper"
        messageLimit="10"
        interval="00:01">
    <target name="mail"
            xsi:type="Mail"
            html="true"
            addNewLines="true"
            replaceNewlineWithBrTagInHtml="true"
            encoding="UTF-8"
            subject="Проблемы с очередями (${var:appname} ${environment:ASPNETCORE_ENVIRONMENT})"
            to="${var:mailto}"
            from="${var:mailfrom}"
            smtpUserName="${var:smtplogin}"
            smtpPassword="${var:smtppassword}"
            smtpServer="${var:smtpserver}"
            smtpPort="465"
            smtpAuthentication="Basic"
            timeout="10000"
            enableSsl="true"
            header="${message}${newline}"
            body="${longdate} ${logger} ${level:upperCase=true} ${message}${onexception:${newline}${exception:format=tostring}} ${newline}"
            footer="Имя машины: ${machinename}${newline}${newline}PID: ${processid}${newline}" />
</target>
```

### Асинхронное логирование

Логирование - это операция ввода-вывода. Она долгая, может замедлять работу приложения. 
Поэтому логирование должно быть асинхронным.

Для этого в секции `target` нужно включить асинхронный режим:

```xml
<targets async="true">
    <target xsi:type="Console"
            name="console"
            layout="${longdate} ${level:upperCase=true} ${message}${onexception:${newline}${exception:format=tostring}}"
            detectConsoleAvailable="true" />
</targets>
```

### Изменение конфига без перезапуска приложения

Может потребоваться изменить уровни логирования, добавить новый `target`, при это не останавливая приложение. 
`NLog` это делает из коробки. Для этого нужно добавить атрибут `autoReload="true"`

```xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true">
</nlog>
```

### Неверный конфиг `NLog`

Если в конфиге есть ошибка, понять это бывает сложно. Но можно:

1. Настроить внутренне логирование (а вот [статья](https://github.com/NLog/NLog/wiki/Internal-Logging), как это сделать)
2. Включить выброс исключения в случае ошибки конфига:
```xml

<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      throwExceptions="true">
</nlog>
```