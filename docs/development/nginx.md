# nginx

Для работы веб части за `nginx` нужно настроить как сам `nginx`, так и веб приложения

> Требуется актуализация, т.к. Let's Encrypt уже не используется.

## Базовая настройка nginx

В каталоге `/etc/nginx/site-available` под каждый сайт создается свой файл. 
Ниже пример `default`, в котором всякая всячина:

```nginx

server {
    listen        80;
    server_name   devsharp.siisltd.ru;

    # LetsEnscrypt
    include acme;
}

server {
    listen        443 ssl http2;
    server_name   devsharp.siisltd.ru;

    client_max_body_size 250m;

    include snippets/ssl.conf;

    # Dummy WebAPI
    location /dummy/api/ {
        proxy_pass         http://127.0.0.1:56154/api/;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }

    # Dummy WebAPI swagger
    location /dummy/api/swagger/ {
        proxy_pass         http://127.0.0.1:56154/swagger/;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}

```

Важно добавлять `include acme;` для поддержки проверки сертификатов.

Важно в `proxy_pass` в конце добавлять `/`, чтобы убирался `location`. 
Также нужно обязательно устанавливаться заголовки `X-Forwarded-For` и `X-Forwarded-Proto`.

Конфиги для `rc` и `dev` окружения находятся в этой директории.

Чтобы активировать сайт, нужно создать `symlink` на файл конфигурации сайта и поместить его в каталог `sites-enaibled` 
и выполнить `systemctl reload nginx`.

Для настройки https используется `letsencrypt` и бот для автоматического продлодления сертификатов `certbot` 
(инструкция [тут](https://habr.com/en/post/318952/)).

Команда для обновления сертификатов во всех доменах:
```bash
sudo certbot certonly -d devsharp.siisltd.ru -d devdo.survey-studio.com -d devapi.survey-studio.com -d rcdo.survey-studio.com -d rcapi.survey-studio.com -d devdex1.survey-studio.com -d devdex2.survey-studio.com -d rcdex1.survey-studio.com -d rcdex2.survey-studio.com -d devwsu.survey-studio.com -d rcwsu.survey-studio.com --expand
```

## Балансировка нагрузки

nginx - это не только proxy сервер, но и балансировщик нагрузки.

Ниже приведени пример настройки nginx с балансировкой для WebData (приведен полный кофниг с комментариями):

```
# Web Data (dev)

## тут нет ничего интересно по балансировки
server {
    listen        80;
    server_name   devdo.survey-studio.com;
    
    autoindex off;

    include /etc/nginx/global.d/*.conf;
    
    location ~ /\.ht {
        deny all;
    }
    
    access_log /var/log/nginx/devdo-ss/access;
    error_log /var/log/nginx/devdo-ss/error error;

    location / {
            return 301 https://$host$request_uri;
    }
}

# определяем список node нашего кластера и то, как распределяются запросы
upstream webdata_hosts {
   # запросы будут распределяеться по хэшу IP (используются первые 3 октета IP)
   ip_hash; 
   # еще есть:
   # - round-robin: тут все банально
   # - least_conn: отдаем тому, у кого меньше соединений 
   # - random: великий рандом  
   # - hash: можно определить свой алгоритм хэшировния на основе чего-либб
   # В платной версии есть:
   # - least_time: отдаем тому, у кого время отклика меньше и меньше соединений
   # - также есть в платной версии есть sticky - позволяет закрепить пользователя за конкретным сервером 
   # (например, если мы используем random, но нам важно, чтобы запросу от пользователя шли на один и тот же сервер)
   server 127.0.0.1:6785; # node0
   server 127.0.0.1:6786; # node1

   # если нам надо перезагрузить сервер, но не нарушать сильно распределение, то надо перед остановкой сервера добавить 
   # в конфиг команду down, чтобы запросы от остановленного сервера шли по другим сервакам, но старые запросы шли к старым сервакам
}

server {
    listen        443 ssl http2;    
    listen [::]:443 ssl http2;
    
    server_name   devdo.survey-studio.com;
    autoindex off;
    
    
    access_log /var/log/nginx/devdo-ss/access;
    error_log /var/log/nginx/devdo-ss/error error;

    client_max_body_size 250m;

    include /etc/nginx/snippets/ssl_common.conf;
    include /etc/nginx/snippets/ssl_surveystudio.conf;
    include /etc/nginx/global.d/*.conf; 



    location / {
        proxy_pass         http://webdata_hosts; # вот тут указываем группу серверов
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;

        proxy_connect_timeout 300s;
        proxy_send_timeout 300s;
        proxy_read_timeout 300s;

        # а вот тут указываем, в каких случаях нужно попробовать отправить запрос на следующий сервер
        # по умолчанию выставляется error и timeout, можно указат свои (ссылки на примеры ниже)
        proxy_next_upstream error timeout http_502;
        
        fastcgi_send_timeout 300s;
        fastcgi_read_timeout 300s;
    }
}

```

Подробнее почитать можно тут:

- [Базовая настройка балансировки](https://nginx.org/en/docs/http/load_balancing.html)

- [Настройка группы серверов](https://nginx.org/en/docs/http/ngx_http_upstream_module.html#server)

- [Настройка перевода запроса на другой сервер в случае ошибки](https://nginx.org/en/docs/http/ngx_http_proxy_module.html#proxy_next_upstream)
