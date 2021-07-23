# EntityFramework

> Раздел в разработке [15.02.2021]

### Не ставить `;` при выполнении собственных запросов средствами EF.

При использовании методов `DbSet<T>.FromSqlRaw()` и `DbSet<T>.FromSqlInterpolated()` в передаваемых им SQL запросах нельзя использовать `;` в конце запроса.
```csharp
var entity = await context.MyEntities
    .FromSqlRaw("SELECT table.* FROM table WHERE table.column > {0}", filteringArg)
    .OrderBy(x => x.Created)
    .ToListAsync();
```
***Почему: EF использует переданный SQL запрос в качестве вложенного и формирует запрос вида `SELECT table.* FROM (YOUR_RAW_QUERY) WHERE ... ORDER  BY ...`, и если в конце вложенного запроса поставить `;`, то можно долго искать ошибку синтаксиса.***
