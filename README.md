# UVS Test
Build a console app which will add and get employees from a database using ORM (e.g. Entity Framework).

## Will test

 * Basic application architecture
 * Testability
 * Attention to detail

## Language

C#

## Requirements

 * Docker or a local sql instance
 * Dotnet core
 * powershell


# UVS‑NET‑TASK

&#x20;

Консольное приложение на **.NET 6** для управления данными сотрудников (добавление и получение) в базе данных PostgreSQL с использованием **Entity Framework Core**.

---

## 📋 Оглавление

- [🚀 Требования](#-требования)
- [🛠 Установка и запуск](#-установка-и-запуск)
  - [1️⃣ Подготовка БД](#1️⃣-подготовка-бд)
  - [2️⃣ Конфигурация](#2️⃣-конфигурация)
  - [3️⃣ Запуск приложения](#3️⃣-запуск-приложения)
- [🎛 Доступные команды](#-доступные-команды)
- [✅ Выполнено](#-выполнено)
- [📝 Лицензия](#-лицензия)

---

## 🚀 Требования

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (для PostgreSQL) или локальный инстанс PostgreSQL
- PowerShell Core или Windows PowerShell

---

## 🛠 Установка и запуск

### 1️⃣ Подготовка БД

1. Откройте PowerShell и перейдите в корень проекта:
   ```powershell
   cd E:\UVS-NET-TASK\Test
   ```
2. Запустите скрипт развёртывания PostgreSQL в Docker:
   ```powershell
   .\setUpDatabase.ps1
   ```
   - Контейнер будет доступен на порту **7777** → **5432**
   - Схема и таблица `employees` создаются из `dbSchema.sql`.

### 2️⃣ Конфигурация

По умолчанию строка подключения хранится в **appsettings.json**:

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=7777;Database=uvsproject;Username=postgres;Password=guest"
}
```

При необходимости измените параметры:

- `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB` в `setUpDatabase.ps1`
- Строку подключения в `appsettings.json`

### 3️⃣ Запуск приложения

Перейдите в папку с проектом и выполните команды:

- **Инициализация БД** (альтернатива PowerShell):
  ```bash
  dotnet run -- init-db
  ```
- **Добавление сотрудника**:
  ```bash
  dotnet run -- set-employee --employeeId 5 --employeeName Steve --employeeSalary 1234
  ```
- **Получение сотрудника**:
  ```bash
  dotnet run -- get-employee --employeeId 5
  ```

---

## 🎛 Доступные команды

| Команда        | Описание                             | Параметры                                                                 |
| -------------- | ------------------------------------ | ------------------------------------------------------------------------- |
| `init-db`      | Инициализация БД и накатывание схемы | —                                                                         |
| `set-employee` | Добавление/обновление сотрудника     | `--employeeId` (int)`--employeeName` (string)`--employeeSalary` (decimal) |
| `get-employee` | Получение данных сотрудника по ID    | `--employeeId` (int)                                                      |

---

## ✅ Выполнено

- Поднятие Docker-контейнера PostgreSQL и накатывание схемы через `setUpDatabase.ps1`
- Консольное приложение на **.NET 6** с **Entity Framework Core** и **Npgsql**
- Команды `` и `` через **System.CommandLine**
- Команда `` для инициализации базы из приложения
- Автоматическая проверка через `verifySubmission.ps1`

---

## 📝 Лицензия

Проект лицензирован под лицензией **MIT**.\
См. файл [LICENSE](LICENSE).

