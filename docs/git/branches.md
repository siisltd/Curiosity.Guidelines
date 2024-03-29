# Ветки в git

Для общего порядка применяются следующие правила в наименовании веток.

- все ветки именуются с нижнем регистре
- используется `kebab-case` (слова в нижнем регистре, разделяются тире)
- наименование основной ветки - `master`
- наименование основной ветки разработки - `dev`
- ветки именуются на английском языке
- опционально можно придерживаться `git flow`

## Git Flow

`git-flow` — это набор расширений git предоставляющий высокоуровневые операции над репозиторием для поддержки [модели ветвления Vincent Driessen](https://nvie.com/posts/a-successful-git-branching-model/). Эта модель ветвления позволяет сделать репозиторий более понятным для любого разработчика. Подробнее прочитать о `git flow` можно по [ссылке](https://danielkummer.github.io/git-flow-cheatsheet/index.ru_RU.html).

Основные моменты:

- `master` - стабильная ветка разработки: тут находится код, который прошел тестирование и развернут на `production`, код в этой ветке обязан компилироваться и проходить все тесты
- `dev` - основная ветка разработки: любые новые фичи и исправления начинаются здесь и вливаются обратно
- `feature/%branch_name%` - отдельные ветки под каждую новую фичу: код тут может быть не компилируемым. Когда разработка заканчивается, делается `merge request` в ветку `dev`, куда изменения попадают после прохождения `code review` (если требуется).
    
    > Почему: следуем принятой в индустрии практики `git-glow`, сразу понятно, что в рамках этой ветки ведутся обычные, плановые работы.

- `hotfix/%branch_name%` - на случай срочных исправлений в продакшене. После вливаются в `master` и в `dev`, также помечается тегом.

    > Почему: следуем принятой в индустрии практики `git-glow`, сразу понятно, что в рамках этой ветки ведутся срочные работы.

- `release/%branch_name%` - ветка для подготовки сборки к релизу: должны прогнаться все тесты, подняться версия сборки, заполниться `changelog`, вся документация должна быть обновлена. Потом изменения вливаются в `master` и `dev`, коммит помечается тегом с номером версии

    > Почему: следуем принятой в индустрии практики `git-glow`, есть отдельная ветка с версией приложения, в которой ведется подготовка к релизу.
