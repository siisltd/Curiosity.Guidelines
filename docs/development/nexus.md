# Nexus

У нас развернут свой nuget репозиторий на базе `Nexus` (https://nexus.siisltd.ru).

`Адрес nuget-ленты`: [https://nexus.siisltd.ru/repository/nuget-hosted/](https://nexus.siisltd.ru/repository/nuget-hosted/)

Чтобы начать использовать эту ленту, в `Rider` нужно выполнить следующие действия (для `VS` будут аналогичны):

- Перейти в настройки `NuGet` (`Tools` -> `NuGet` -> `NuGet Settings`) и в разделе `Credential Providers` выбрать `None`
- Открыть вкладку `NuGet`
- Открыть вкладку `Sources`
- Выбрать конфигурацию, открыть файл на редактирование (двойной клик по элементу из списка), заменить следующим содержимым:
  
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
<packageSources>
  <add key="siisltd" value="https://nexus.siisltd.ru/repository/nuget-hosted/"/>
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
</packageSources>
<packageRestore>
  <add key="enabled" value="True" />
  <add key="automatic" value="True" />
</packageRestore>
<bindingRedirects>
  <add key="skip" value="False" />
</bindingRedirects>
<packageManagement>
  <add key="format" value="0" />
  <add key="disabled" value="False" />
</packageManagement>
<packageSourceCredentials>
  <siisltd>
    <add key="Username" value="USER" />
    <add key="ClearTextPassword" value="PASSWORD" />
  </siisltd>
</packageSourceCredentials>
</configuration>
```
- задать свои логин/пароль
- сохранить изменения.

Анонимные подключения к `Nexus` запрещены в целях безопасности.

Для получения аккаунта обратиться к [Максиму](https://t.me/markeli).

Для настройки `NuGet` на build-машине нужно обновить таким же образом файлы конфигурации, указав необходимые креды. 

## Ссылки
- [Настройка NuGet](https://docs.microsoft.com/ru-ru/nuget/consume-packages/configuring-nuget-behavior)
- [Файл NuGet.Config](https://docs.microsoft.com/en-us/nuget/reference/nuget-config-file)