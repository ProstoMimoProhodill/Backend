# Backend сайта GraphLabs

## Реализованные и запланированные фичи
- [x] Список модулей
- [x] Список вариантов
- [x] Выдача случайного варианта
- [x] Выдача файлов модуля
- [x] База общих изображений
- [ ] Выдача псевдо-случайного варианта
- [x] Пользователи
- [x] Аутентификация
- [ ] Авторизация по ролям
- [X] Научить логи заданий получать пользователя из данных аутентификации
- [ ] Logout
- [x] Лог выполнения

## Технологии
* Asp.Net Core
* OData Core
* EntityFramework Core

## Подготовка к первому запуску
* Установите Visual Studio или Rider последней версии
* Установите [Net Core SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2)
* Установите сертификат для отладки на localhost'е через https, выполнив:
`dotnet dev-certs https --trust`
* Подключение к БД настраивается переменными окружения:
  * `DB_HOST=postgres.example.com`
  * `DB_NAME=graphlabs`
  * `DB_USER=graphlabs`
  * `DB_PASSWORD=my_password`

## Аутентификация

> **POST** http://localhost:5000/auth/login

_Content-Type:_ application/json

_Body:_
```json 
{
    "email": "student-1@graphlabs.ru",
    "password": "первый"
}
```

Ответ будет примерно такой:
```json
{
    "userId": 0,
    "firstName": "Администратор",
    "lastName": "Администратор",
    "token": "бла-бла-бла"
}
```

Во все последующие запросы необходимо включать заголовок

_Authorization_ : "Bearer бла-бла-бла", где бла-бла-бла - тот самый token, полученный в ответе 

**Renew токена**
По-умолчанию, токен действителен 1 день с момента выдачи (или до перезапуска сервера). Но его возможно обновить:
> **POST** http://localhost:5000/auth/renew
Ответ аналогичен запросу логина, плюс придёт новый токен.


## Примеры использования
**Метаданные:**
> **GET** http://localhost:5000/odata/$metadata

**Список всех модулей-заданий:**
> **GET** http://localhost:5000/odata/taskModules

**Модуль с идентификатором 1:**
> **GET** http://localhost:5000/odata/taskModules(1)

**Запрос с фильтрацией (название = "КСС"):**
> **GET** http://localhost:5000/odata/taskModules?$filter=Name eq 'КСС'

**Запрос с фильтрацией, лимитом и упорядочиванием:**
> **GET** http://localhost:5000/odata/taskModules?$filter=version eq '2.0'&$top=3&$orderby=Name desc

**Запрос случайного варианта задания с id = 2:**
> **GET** http://localhost:5000/odata/taskModules(2)/randomVariant

**Скачивание файла "service-worker.js" модуля-задания с id = 1:**
> **GET** http://localhost:5000/odata/taskModules(1)/download(path='service-worker.js')

**Скачивание файла "static/main.a091e228.js" модуля-задания с id = 1:**
> **GET** http://localhost:5000/odata/taskModules(1)/download(path='static%2Fjs%2Fmain.a091e228.js')

**Скачивание изображения "Complete.png" из общей библиотеки:**
> **GET** http://localhost:5000/odata/downloadImage(name='Complete.png')

**Список всех вариантов:**
> **GET** http://localhost:5000/odata/taskVariants

**Json варианта с идентификатором 5:**
> **GET** http://localhost:5000/odata/taskVariants(5)/json

**Обновить вариант с идентификатором 5:**
> **POST** http://localhost:5000/odata/taskVariants(5)

Для создания нового варианта передайте идентификатор 0 и в URL и в теле (meta/id=0). 

_Content-Type:_ application/json

_Body:_
```json
{
  "data": [
    {
      "type": "graph",
      "value": {
        "vertices": [
          "1",
          "2",
          "3",
          "4",
          "5"
        ],
        "edges": [
          {
            "source": "1",
            "target": "2"
          },
          {
            "source": "2",
            "target": "3"
          },
          {
            "source": "3",
            "target": "4"
          },
          {
            "source": "4",
            "target": "5"
          },
          {
            "source": "5",
            "target": "1"
          }
        ]
      }
    }
  ],
  "meta": {
    "name": "Вариант 5",
    "id": "5",
    "moduleId": "2"
  }
}
```
> в ответе придёт json ```{"@odata.context":"http://localhost:5000/odata/$metadata#Edm.Int64","value":1}```,
> где value - идентификатор созданного/обновлённого варианта

**Удалить вариант с идентификатором 22:**
> **DELETE** http://localhost:5000/odata/taskVariants(22)
>
**Список всех студентов:**
> **GET** http://localhost:5000/odata/students

**Студент с идентификатором 2:**
> **GET** http://localhost:5000/odata/students(2)

**Студент с почтой "student-3@graphlabs.ru":**
> **GET** http://localhost:5000/odata/students?$filter=email eq 'student-3@graphlabs.ru'

**Список всех действий:**
> **GET** http://localhost:5000/odata/taskVariantLogs

**Зарегистрировать новое действие:**
> **POST** http://localhost:5000/odata/taskVariantLogs
_Content-Type:_ application/json
_Body:_
```json
{
    "action": "описание действия",
    "variantId": "Id варианта",
    "penalty": 10
}
```

**Загрузить новую версию модуля c идентификатором 1:**
> **POST** http://localhost:5000/odata/taskModules(1)/upload 
_Body:_
```binary data: zip archive```

**Запросить информацию о текущем пользователе:**
> **GET** http://localhost:5000/odata/currentUser