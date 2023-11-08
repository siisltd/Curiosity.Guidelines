# Conventions

## C# Coding Standards and Naming Conventions

| Названия объекта              | Нотация   | Длина | Множественное число | Префиксы | Суффиксы | Аббревиатуры** | Используемые символы          | Подчеркивания |
|:--------------------------|:-----------|-------:|:-------|:-------|:-------|:-------------|:-------------------|:------------|
| Класс                | PascalCase |    128 | Нет     | Нет     | Да    | Нет           | [A-z][0-9]         | Нет          |
| Конструктор          | PascalCase |    128 | Нет     | Нет     | Да    | Нет           | [A-z][0-9]         | Нет          |
| Метод               | PascalCase |    128 | Да    | Нет     | Нет     | Нет           | [A-z][0-9]         | Нет          |
| Аргументы метода          | camelCase  |    128 | Да    | Нет     | Нет     | Да          | [A-z][0-9]         | Нет          |
| Локальные переменные           | camelCase  |     50 | Да    | Нет     | Нет     | Да          | [A-z][0-9]         | Нет          |
| Константы            | PascalCase |     50 | Нет     | Нет     | Нет     | Нет           | [A-z][0-9]         | Нет          |
| Поля                | camelCase  |     50 | Да    | Нет     | Нет     | Да          | [A-z][0-9]         | Да         |
| Свойства           | PascalCase |     50 | Да    | Нет     | Нет     | Да          | [A-z][0-9]         | Нет          |
| Делегаты             | PascalCase |    128 | Нет     | Нет     | Да    | Да          | [A-z]              | Нет          |
| Enum            | PascalCase |    128 | Да    | Нет     | Нет     | Нет           | [A-z]              | Нет          |

> ***Аббревиатуры можно использовать, если они приняты в индустрии и у нас в компании.***

### Использовать PascalCasing для имен классов и методов

```csharp
public class ClientActivity
{
  public void ClearStatistics()
  {
    //...
  }
  public void CalculateStatistics()
  {
    //...
  }
}
```

***Почему: следование code-style Microsoft в .NET Framework и упрощение чтения.***

### Использовать camelCasing для аргументов методов и локальных переменных:

```csharp
public class UserLog
{
  public void Add(LogEvent logEvent)
  {
    int itemCount = logEvent.Items.Count;
    // ...
  }
}
```

***Почему: следование code-style Microsoft в .NET Framework и упрощение чтения.***

### Не использовать венгерскую нотацию (Hungarian notation) или другой способ указания типа в названии

```csharp
// Правильно
int counter;
string name;    
// Неправильно
int iCounter;
string strName;
```

***Почему: следование code-style Microsoft в .NET Framework и IDE сама помогает показывать типы с помощью подсказок (например, в VS через InteliSense). Следует в принципе избегать указания типа в любом идентификаторе***

### Не использовать caps для констант и readonly переменных:

```csharp
// Правильно
public const string ShippingType = "DropShip";
// Неправильно
public const string SHIPPINGTYPE = "DropShip";
```

***Почему: следование code-style Microsoft в .NET Framework. Caps привлекает слишком много внимания.***

###Использовать осмысленные названия для переменных. Например seattleCustomers лоя заказчиков из Сиэтла:

```csharp
var seattleCustomers = from customer in customers
  where customer.City == "Seattle" 
  select customer.Name;
```

***Почему: следование здравому смыслу, code-style Microsoft в .NET Framework и упрощение чтения.***

### 6. Избегать использования аббревиатур. Исключения: аббревиатуры, широко используемые как названия, например Id, Xml, Ftp, Uri; наши собственные аббревиатуры (SSNG, DEX)

```csharp    
// Правильно
UserGroup userGroup;
Assignment employeeAssignment;     
// Неправильно
UserGroup usrGrp;
Assignment empAssignment; 
// Exceptions
CustomerId customerId;
XmlDocument xmlDocument;
FtpHelper ftpHelper;
UriPart uriPart;
```

***Почему: следование code-style Microsoft в .NET Framework и предотвращение несогласованных сокращений, чтобы не было разногласий в понимании***

### Использовать PascalCasing для аббревиатур от 3 символов и больше:

```csharp  
HtmlHelper htmlHelper;
FtpTransfer ftpTransfer;
UIControl uiControl;
```
Исключения: SSNG

***Почему: следование code-style Microsoft в .NET Framework. Caps привлекает слишком много внимания.***

### Не использовать нижнее подчеркивание в идентификаторах. Исключения: разделение префикса в приватных полях; naming тестовых методов

```csharp 
// Правильно
public DateTime clientAppointment;
public TimeSpan timeLeft;    
// Неправильно
public DateTime client_Appointment;
public TimeSpan time_Left; 
// Исключение (поле класса)
private DateTime _registrationDate;
```

***Почему: следование code-style Microsoft в .NET Framework.***

### Использовать предопределенные названия типов (C# алиасы), например, `int`, `float`, `string` для локальных переменных и объявления параметров типа. Использовать `String` при доступе к статическим полям класса: or `String.Join`

```csharp
// Правильно
string firstName;
int lastIndex;
bool isSaved;
string commaSeparatedNames = String.Join(", ", names);
int index = Int32.Parse(input);
// Неправильно
String firstName;
Int32 lastIndex;
Boolean isSaved;
string commaSeparatedNames = string.Join(", ", names);
int index = int.Parse(input);
```

***Почему: следование code-style Microsoft в .NET Framework и поддержка более читаемого кода.*** 

### Использовать `var` для определения типа локальных переменных

```csharp 
var stream = File.Create(path);
var customers = new Dictionary();
var index = 100;
string timeSheet;
bool isCompleted;
```

***Почему: удаляет беспорядок, особенно со сложными универсальными типами. Тип легко подсказывает IDE. Более легкий рефакторинг.***

### Использовать существительные или именные фразы для названий классов

```csharp 
public class Employee
{
}
public class BusinessLocation
{
}
public class DocumentCollection
{
}
```

***Почему: следование code-style Microsoft в .NET Framework и более легкое запоминание.***

### Добавлять к интерфейсу префикс в виде букву I. Имена интерфейсов - существительные (или) или прилагательные.

```csharp     
public interface IShape
{
}
public interface IShapeCollection
{
}
public interface IGroupable
{
}
```

***Почему: следование code-style Microsoft в .NET Framework.***

### Называть файлы так же, как и основной класс в файле. Исключения: названия файлов с `partial` классами должны отражать свою специфику, например. designer, generated, и т.д.. В целом один публичный класс - один файл.

```csharp 
// Расположен в Task.cs
public partial class Task
{
}
// Расположен Task.generated.cs
public partial class Task
{
}
```

***Почему: соответствует практикам Microsoft. Файлы сортируются в проекта в алфавитном порядке, partial классы идут рядом.***

### Namespace должны обладать четко понятной структурой: 

```csharp 
// Examples
namespace Company.Product.Module.SubModule
{
}
namespace Product.Module.Component
{
}
namespace Product.Layer.Module.Group
{
}
```

***Почему: следование code-style Microsoft в .NET Framework. Поддержка хорошей организации проекта.***

### Фигурные скобки выравнивать вертикально: 

```csharp 
// Правильно
class Program
{
  static void Main(string[] args)
  {
    //...
  }
}
```

***Почему: Мы не на Java пишем***

### Придерживаться следующей структуры классов:

- константы и static readonly поля
- все зависимости класса
- все поля класса
- все свойства класса
- делегаты и события
- конструктор
- методы.

```csharp 
// Правильно
public class Account
{
  #region Константы

  private const int MaxValue = 10;

  public static readonly int EntityTypeId = 123;

  # endregion

  #region Зависимости

  private readonly IDependency _dependecy;

  #endregion

  #region Поля

  private static string _bankName;
  private static decimal _reserves;      

  #endregion

  #region Свойства

  public string Number { get; set; }
  public DateTime DateOpened { get; set; }
  public DateTime DateClosed { get; set; }
  public decimal Balance { get; set; }     

  #endregion

  #region Делегаты и события

  public event EventHandler SomethingHappened;

  #endregion

  // Constructor
  public Account()
  {
    // ...
  }

  // тут идут методы
}
```

***Почему: Общепринятая практика, которая предотвращает необходимость поиска объявлений переменных и прочего.***

### Использовать свойства, а не поля класса

Свойства имеют больше преимуществ:

- изменение функциональности без изменения API;
- возможность определить свойства в интерфейсе;
- свойства могут быть readonly для внешних потребителей;
- возможность поставить break point в коде свойства:
- внутри них можно вызывать события

С точки зрения производительности, свойства совсем незначительно отличаются от полей класса. Пруф есть в разделе [Производительность](./performance.md)


### Использовать слова в единственном числе для enum. Исключение: битовые маски.

```csharp 
// Правильно
public enum Color
{
  Red,
  Green,
  Blue,
  Yellow,
  Magenta,
  Cyan
} 
// Исключение
[Flags]
public enum Dockings
{
  None = 0,
  Top = 1,
  Right = 2, 
  Bottom = 4,
  Left = 8
}
```

***Почему: следование code-style Microsoft в .NET Framework, создание более читаемого кода. Для флагов используется множественная форма, потому что они хранят несколько значений.***

### Не указывать явно тип, от которого наследуется enum (исключение - битовые флаги):

```csharp 
// Неправильно
public enum Direction : long
{
  North = 1,
  East = 2,
  South = 3,
  West = 4
} 
// Правильно
public enum Direction
{
  North,
  East,
  South,
  West
}
```

***Почему: может создавать путаницу при использовании фактических типов и значений***

### Не используйте суффикс "Enum" в именах типов перечислений:

```csharp     
// Неправильно
public enum CoinEnum
{
  Penny,
  Nickel,
  Dime,
  Quarter,
  Dollar
} 
// Правильно
public enum Coin
{
  Penny,
  Nickel,
  Dime,
  Quarter,
  Dollar
}
```

***Почему: следование code-style Microsoft в .NET Framework и в соответствии с предыдущим правилом отсутствия индикаторов типа в идентификаторах.***

### Не используйте суффиксы "Flag" или "Flags" в именах типов перечислений:

```csharp 
// Не использовать
[Flags]
public enum DockingsFlags
{
  None = 0,
  Top = 1,
  Right = 2, 
  Bottom = 4,
  Left = 8
}
// Правильно
[Flags]
public enum Dockings
{
  None = 0,
  Top = 1,
  Right = 2, 
  Bottom = 4,
  Left = 8
}
```

***Почему: следование code-style Microsoft в .NET Framework и в соответствии с предыдущим правилом отсутствия индикаторов типа в идентификаторах.***

### Проверка флагов через метод `HasFlag`:

```csharp 
// Не использовать
var hasFlag = (questionFlags & QuestionFlags.HidePreviousButton) != 0;

// Правильно
questionFlags.HasFlag(QuestionFlags.HidePreviousButton)
```

***Почему: Более читаемый код.***

> Раньше советовали делать проверку наличия флага через битовую операцию, т.к. работало быстрее, чем HasFlag. Но в .NET Core 3.1 это изменилось, HasFlag работает также быстро (пруф: https://www.code4it.dev/blog/hasflag-performance-benchmarkdotnet).

### Для enum'ов, которые сохраняются в базу или передаются по сети, нужно первым делать значение `Unknown=0` или `None=0`, для остальных значений проставлять числовые значения:

```csharp 
// Правильно
public enum Dockings
{
  None = 0,
  Top = 1,
  Right = 2, 
  Bottom = 4,
  Left = 8
}
```

***Почему: поддержка конситентности значений.***

### Использовать суффикс EventArgs при создании новых классов, содержащих информацию о событии:

```csharp 
// Правильно
public class BarcodeReadEventArgs : System.EventArgs
{
}
```

***Почему: следование code-style Microsoft в .NET Framework и упрощение чтения.***

### Добавлять названиям обработчикам событий (делегатам, используемые в качестве типов событий) суффикс "EventHandler":

```csharp 
public delegate void ReadBarcodeEventHandler(object sender, ReadBarcodeEventArgs e);
```

***Почему: следование code-style Microsoft в .NET Framework и упрощение чтения.***

### Не создавать имена параметров в методах (или конструкторах), которые отличаются только регистром:

```csharp 
// Неправильно
private void MyFunction(string name, string Name)
{
  //...
}
```

***Почему: следование code-style Microsoft в .NET Framework, легкое чтение и отсутствия конфликта в понимании.***


### Использовать суффикс Exception при создании новых классов, содержащих информацию об исключении:

```csharp 
// Правильно
public class BarcodeReadException : System.Exception
{
}
```

***Почему: следование code-style Microsoft в .NET Framework и упрощение чтения.***

### При использовании `fluent api` (длинная цепочка вызовов, `LINQ`) отдельные методы писать с новой строки:

```csharp 
// Правильно
public void UpdateData()
{
  var (using context = _contextFactory.Create())
  {
    var data = context
      .Data
      .Where(x => x.Id > 100)
      .ToList();

    // и что-то еще делается
  }
}
```
***Почему: Легкое чтение, легкий merge.***

### Если аргументов больше 2 или у них длинные названия типов и самого аргумента, переносить каждый на новую строку:

```csharp 
// Правильно
public void UpdateData(
  [NotNull] NyLongNameData data,
  [NotNull] SomethingTerrable terrableData,
  int index,
  DateTime timestamp
)
{
    // и что-то еще делается
}
```

***Почему: упрощение чтения, быстрое разделение аргументов глазами, легкий merge.***

### Логические проверки с множеством условий разбивать построчно:

```csharp 
// Правильно
if (condition1 &&
    condition2 &&
    condition3)
  {
      // что-то делаем
  }
```

***Почему: упрощение чтения, быстрое разделение аргументов глазами, легкий merge.***

### В параметрах аргумента аттрибут идет рядом с типом, при объявлении результата значения метода, свойства или поля класса - сверх:

```csharp 

// Правильно
[NotNull]
private Data _data;

// Неправильно
[CanBeNull] private Data _oldData;

// Правильно
[CanBeNull]
public Data UpdateData(
  [NotNull] NyLongNameData data,
  [NotNull] SomethingTerrable terrableData,
  int index,
  DateTime timestamp
)
{
    // и что-то еще делается
}
```

***Почему: упрощение чтения***


### Для асинхронных методов добавлять суффикс Async:

```csharp 

// Правильно
public Task<Data> UpdateDataAsync(
  [NotNull] NyLongNameData data,
  [NotNull] SomethingTerrable terrableData,
  int index,
  DateTime timestamp
)
{
    // и что-то еще делается
}
```

***Почему: следование code-style Microsoft в .NET Framework и упрощение чтения.***


### Все аргументы публичных методов и конструкторов должны проверяться:

```csharp 

// Правильно
public Task<Data> UpdateDataAsync(
  [NotNull] NyLongNameData data,
  [NotNull] SomethingTerrable terrableData,
  int index,
  DateTime timestamp
)
{
  if (data == null) throw new ArgumentNullException(nameof(data));
  if (!data.IsReady) throw new InvalidOperationException(nameof(data));
    // и что-то еще делается
}
```

***Почему: Безопасность - превыше всего. Метод гарантированно работает, если с данными все ок. Иначе падает сразу.***


### Методы стоит располагать в порядке их вызова друг другом

```csharp 

// Правильно

public void DoFirst()
{
  // some logic
  PrepareData();
  // yet another logic
  DoLast();
}

private void PrepareData()
{
  // some logic
}

public void DoLast()
{
  // some logic
}
```

***Почему: Код легко читается, логика потока выполнения более четкая***

### Код должен быть задокументирован

Как минимум, все публичные контракты.
Следуем гайду от [Microsoft](https://docs.microsoft.com/ru-ru/dotnet/csharp/codedoc)

Также оставляем комменты внутри методов.

***Не каждый поймет нашу гениальность.***


### Между блоками кода должен быть максимум отступ в одну строку

Двойные и более пустые строки - недопустимы, а одиночные - нужны для смыслового выделения каких-то моментов.

***Создается ощущение неаккуратности или что был вырезан фрагмент кода, не факт что верно, либо что какой-то акцент важный, что тоже пугает.***

### Не использовать однострочный `using`

В C# 8.0 появилась возможность вместо блоки `using` делать однострочный `using`.

Когда мы используем `using` по-старинке, то у нас метод `Dispose` будет вызван в конце блока `using`. С однострочным `using` конец блока растягивается до конца текущей области видимости, т.е. обычно до конца метода. Это плохо, ресурсы нужно освобождать сразу, как они перестали нам быть нужны. 

Сокращение уровня вложенности кода за счет менее эффективного использование ресурсов - сомнительная оптимизация. Использовать однострочную версию только в крайней необходимости.

### Ссылки

1. [MSDN General Naming Conventions](http://msdn.microsoft.com/en-us/library/ms229045(v=vs.110).aspx)
2. [DoFactory C# Coding Standards and Naming Conventions](http://www.dofactory.com/reference/csharp-coding-standards)
3. [MSDN Naming Guidelines](http://msdn.microsoft.com/en-us/library/xzf533w0%28v=vs.71%29.aspx)
4. [MSDN Framework Design Guidelines](http://msdn.microsoft.com/en-us/library/ms229042.aspx)