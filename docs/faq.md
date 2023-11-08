# FAQ

Часто встречающиеся вопросы и способы их решения вынесены в `FAQ`. Все решения записывать в этой статье.

Для удобства поиска вопросов есть определенный список `labels`:

- `DevOps`: все, связанное с настройкой окружения на сервере
- `Docker`: работа с `Docker`
- `Rider`: все, что связано с `JetBrains Rider`
- `IDE`: проблемы с IDE в целом (`Rider`, `Visual Studio`, `VS Code`)
- `Opened`: вопрос не решен (если решен, то этот тег не указывать)

Формат описания проблемы:

## Короткое описание проблемы

`Tags`: `указываем тег для быстрого поиска`

`Q:` тут подробно описываем свою проблему

`A:` тут описываем решение

## Не запускается CI

`Tags:` `DevOps`

`Q:` CI сконфигурирован правильно, но изменения из гита не подтягиваются
Вывод в логе примерно такой:
```
Running with gitlab-runner 12.2.0 (a987417a)
  on .NET Core CI machine BshuJcsL
Using Shell executor...
Running on gitlab-runners...
bash: line 81: cd: /home/gitlab-runner/builds/BshuJcsL/0/wciom/u-cati: No such file or directory
ERROR: Job failed: exit status 1
```
`A:` [Инструкция](https://gitlab.com/gitlab-org/gitlab-runner/issues/1379#note_109693923): важно запустить в режиме дебага.

После нужно перезапустить службы:
```
sudo systemctl daemon-reload
sudo systemctl restart gitlab-runner
```

Пример рабочего `systemd` файла:

```
[Unit]
Description=GitLab Runner
After=syslog.target network.target
ConditionFileIsExecutable=/usr/lib/gitlab-runner/gitlab-runner

[Service]
StartLimitInterval=5
StartLimitBurst=10
ExecStart=/usr/lib/gitlab-runner/gitlab-runner --debug "run" "--working-directory" "/home/gitlab-runner" "--config" "/etc/gitlab-runner/config.toml"
User=gitlab-runner
Group=gitlab-runner
WorkingDirectory=/home/gitlab-runner
StandardInput=tty-force
StandardOutput=inherit
StandardError=inherit


Restart=always
RestartSec=120

[Install]
WantedBy=multi-user.target
```
> Это решение работает только для версии раннера `12.2.0`.

## Запуск под Windows. MSBuild targets were not found.

`Tags:` `IDE`, `Rider`

`Q`: При попытке запустить проект в Rider под Windows возникает ошибка MSBuild targets were not found.

`A`: Для разрешения этой проблемы придется поставить Visual Studio и, затем, в настройках Rider, указать его MSBuild, а не тот, который поставляется с SDK. Пример:
![IMG](/images/faq_rider_msbuild.png)

## Не запускается интеграционный тест в CI

`Tags:` `DevOps`

`Q`: При прогоне интеграционного теста в `CI` и использовании `docker` контейнера тест падает, ругается на уже существующий контейнер/сеть 

`A`: Проблема возникает при одновременном запуске одинаковых интеграционных тестов в двух ветках (например, автоматом в `dev` и `release` ветках). После такой проблемы тесты в принципе перестают запускаться. Чтобы исправить, нужно:
- зайти на `build` машину
- просмотреть сети docker (`docker network ls`)
- найти сети с одинаковым названием
- удалить сеть (`docker network rm {network name}`)

## Сбивается кодировка русского текста в ASP MVC

`Tags:` `IDE`, `Rider`

`Q`: При создании нового Razor MVC View в Rider - сбивается кодировка кириллических символов при отображении страницы в браузере.

`A`: Rider по-умолчанию создает файлы в UTF-8 без BOM кодов. Это и приводит к некорректному отображению страницы в браузере. Для того, чтобы изменить поведение по-умолчанию нужно изменить следующий параметр:
```
Settings -> Editor -> File Encodings -> Create UTF-8 files: with BOM
```
NOTE: Настройка затрагивает только новые файлы и, как следствие, проблемные страницы придется пересоздать.

## Нет прав на выполнение скрипта на сервере

`Tags:` `DevOps`

`Q`: при запуске скрипта через `CI` появляется ошибка: 
```
bash: line 73: ./deploy.sh: Permission denied
```
`A`: добавить право на выполнение для скрипта, запомнить это право для `git` (выполняется на машине разработчика):
```
sudo chmod +x deploy.sh
sudo git update-index --add --chmod=+x deploy.sh
```