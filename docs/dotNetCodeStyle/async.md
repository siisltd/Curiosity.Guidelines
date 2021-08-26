# Асинхронное и многопоточное программирование

В этой статье собраны рекомендации по написанию асинхронного и многопоточного кода.

Т.к. наша система является нагруженной и многопользовательской, код мы должны писать таким образом, 
чтобы приложения справлялись с нагрузкой. 
Поэтому активно используем асинхронное программирование с помощью любимых `async/await`.

> Важно помнить, что сам по себе `async/await` не делает код более быстрым: смена контекста, `state machine`, все дела.

В веб приложениях польза `async/await` огромна, т.к. позволяет увеличить производительность приложения за счет переиспользования потоков из пула 
во время выполнения `I/O` операций. Поэтому в вебе - все `async`. Либы, которые используются в вебе - тоже `async`.

## Рекомендации

### Весь стек вызова должен быть асинхронным

Если метод вызывает другой асинхронный метод, то он тоже должен быть асинхронным и не 
должен блокировать поток, пока ожидается результат выполнения

```csharp
// неправильно
public int GetCount()
{
    return GetCountInternalAsync().GetAwaiter().GetResult();
}

```

### Асинхронный метод не должен возвращать void

В случае ошибки в асинхронном методе может упасть весь процесс, т.к. ошибка не обрабатывается.

Если очень нужно написать такой метод, то всегда используйте `ContinueWith`:

```csharp
public void LongRunningOperation()
{
    var task = InternalLongRunningOperaion();
    task.ContinueWith(handler, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
}
```

### Использовать `CancellationToken`

Должна быть возможность остановить асинхронную операцию (например, наш асинхронный метод, 
который внутри делает кучу всего). Это нужно при остановке приложения, таймауте.  

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        await Task.Delay(_sleepTime, stoppingToken);
    }
}
```

### Использовать `Task.FromResult`, если нужно синхронный метод сделать асинхронным

Иногда интерфейс требует, чтобы методы был асинхронным, но по факту операция может выполниться синхронно. 
Для этого надо использовать `Task.FromResult`, а не запускать новую таску через `Task.Run`, 
т.к. не будет создаваться новый поток (или браться из пула):

```csharp

// неправильно
public Task<int> GetCountAsync()
{
    return Task.Run(() => _count);
}

// правильно
public Task<int> GetCountAsync()
{
    return Task.FromResult(_count);
}

```
> Если нужно вернуть просто `Task`, то пользуйтесь `Task.FromResult<bool>(true)`, т.к. нет метода `Task.FromResult()`.

### Тюнинг и мониторинг пула потоков

Наши приложения нагружены, поэтому нам нужно сразу настроить пул потоков и следить за их состоянием. 
Все для этого есть в `nuget` пакете `Curiosity.Utils.Hosting`.

> Требуется актуализация пакета.

### Использовать `Task.Yeild`, если асинхронный метод может долго выполняться синхронно перед тем, как будет вызван асинхронный метод

Например, наш асинхронный метод разгребает очередь, построенную на базе `BlockingCollection`. 

```csharp
 protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    // если этого нет, то поток будет заблокирован, пока _notificationQueue.Take не вернет результат
    await Task.Yield();
    

    while (!stoppingToken.IsCancellationRequested)
    {
        var queueItem = _notificationQueue.Take(stoppingToken);

        var notifications = queueItem.Notifications;
       
        await ProcessNotificationAsync(notifications);
    
    }
}
```

При первом вызове метода `ExecuteAsync` вызывающий поток будет заблокирован, пока в очереди не появятся первый элемент. Это плохо.
`Task.Yield` принудительно вызывает смену контекста, что приводит к освобождению вызывающего потока.  

### Не запускать долгие вычислительные операции в отдельном потоке

Асинхронность хороша для выполнения `I/O bound` операций, поэтому использовать поток из пула для вычислительной операции не рекомендуется.
Если очень хочется, то можно запустить с опцией `TaskCreationOptions.LongRunning`:

```csharp

public void StartOperation()
{
    Task.Factory.StartNew(LongRunningOperation, TaskCreationOptions.LongRunning);
}

public void LongRunningOperation()
{
}

```

### Всегда вызывать `FlushAsync`, если асинхронно работаете с любым наследником `Stream`

Даже если используете поток в `using`, всегда это нужно делать, иначе забуферизированные данные не попадут туда, куда мы хотели.

### Если метод внутри только вызывает другой асинхронный метод, не нужно делать на нем `await` 

Мы экономим на пареключении потоков, не создается машина состояния, меньше нагрузка на `GC`.

> Исключение: если вызов обернут в `try-finally`, иначе `finally` отработает некорректно.
> Еще одно исключение: если внутри используется `using`.

```csharp
public Task<int> GetCountAsync()
{
    return GetCountInternalAsync();
}
```

Замеры производительности есть в [этой статье](https://www.tabsoverspaces.com/233659-do-not-await-what-does-not-need-to-be-awaited).

### Не работайте с непотокобезопасным кодом в нескольких потоках

Тут все очевидно.

## Полезные ссылки

- [Про `CPU bound` и `I/O bound`](https://www.infoworld.com/article/3201030/understand-the-net-clr-thread-pool.html)
- [Про контекст синхронизации](http://hamidmosalla.com/2018/06/24/what-is-synchronizationcontext/)
- [Про то, как приложение с async ведет себя под нагрузкой](https://habr.com/en/company/dododev/blog/461081/)