# Systemd

Для запуска приложения как службы на Linux машинах можно пользоваться `systemd`. Для этого нужно в директории `/etc/systemd/system` создать файл конфигурации с расширением `*.service`.

## Пример конфига

```ini
[Unit]
Description=Example .NET Web API App running on Ubuntu

[Service]
WorkingDirectory=/var/www/helloapp
ExecStart=/usr/bin/dotnet /var/www/helloapp/helloapp.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-example
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

> Нужно для `KillSignal` указывать `SIGINT`, т.к. `SIGKILL` нормально не обрабатывается `dotnet` приложениями в данный момент.

Если пользователь www-data в конфигурации не используется, необходимо сначала создать определенного в примере пользователя, а затем предоставить ему необходимые права владения для файлов.

Подробнее о файле конфигурации можно прочитать в [официальной документации](https://docs.microsoft.com/ru-ru/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.2) от Microsoft.

## Основные команды для работы с сервисами

> Можно указывать имя сервиса без .service

- `systemctl list-units`: отображает список сервисов, запущенных на сервере
- `systemctl restart application.service`: перезапуск сервиса
- `systemctl reload application.service`: обновляет логика работы сервиса без полной перезагрузки (невсегда работает, только если сервис может это делать)
- `systemctl start application`: запуск
- `systemctl stop application.service`: остановка
- `systemctl enable application`: запуск сервиса при старте системы (добавим в автозагрузку)
- `systemctl disable application.service`: удалим из автозагрузки
- `status application.service`: проверяем статус сервиса
- `journalctl -u nginx.service --since today`: логи

## Нюансы

Иногда после изменения конфига сервис может не запуститься. Это лечится командой
`systemctl daemon-reload`