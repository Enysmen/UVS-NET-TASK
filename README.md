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

Console application on **.NET 6** for managing employee data (adding and retrieving) in a PostgreSQL database using **Entity Framework Core**.

---

## 📋 Table of contents

- [🚀 Requirements](#-requirements)
- [🛠 Install and run](#-install-and-run)
- [1️⃣ Preparing the database](#1️⃣-preparing-the-database)
  - [2️⃣ Configuration](#2️⃣-configuration)
  - [3️⃣ Launch application](#3️⃣-launch-application)
- [🎛 Available commands](#-available-commands)
- [✅ Completed](#-completed)


---

## 🚀 Requirements

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for PostgreSQL) or a local PostgreSQL instance
- PowerShell Core or Windows PowerShell

---

## 🛠 Installation and launch

### 1️⃣ Preparing the database

1. Open PowerShell and navigate to the project root:
   ```powershell
   cd E:\UVS-NET-TASK\Test
   ```
2. Run the PostgreSQL deployment script in Docker:
   ```powershell
   .\setUpDatabase.ps1
   ```
  - The container will be available on port **7777** → **5432**
  - The `employees` schema and table are created from `dbSchema.sql`.

### 2️⃣ Configuration

By default, the connection string is stored in **appsettings.json**:

```json
{
  "UvsTaskPassword": "guest",
  "UvsTaskDatabase": "uvsproject",
  "UvsTaskPort": "7777",
  "UvsTaskSchemaLocation": "dbSchems/dbSchema.sql",
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=7777;Database=uvsproject;Username=postgres;Password=guest"
  }
}
```

Change the parameters if necessary:

- `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB` в `setUpDatabase.ps1`
- Connection string in `appsettings.json`

### 3️⃣ Launch the application

Go to the project folder and run the commands:

- **Database initialization** (PowerShell alternative):
  ```bash
  dotnet run -- init-db
  ```
  There are two options for starting a database: one is to run the script "setUpDatabase.ps1" where the database will be created, and the other is to run a Docker container.
  
- **Another way to start**:
```bash
  docker run -d `
  --name uvs-postgres `
  -e POSTGRES_USER=postgres `
  -e POSTGRES_PASSWORD=guest `
  -e POSTGRES_DB=uvsproject `
  -p 7777:5432 `
   postgres:latest
  ```
We raise the Docker container and then enter the command "dotnet run -- init-db" and then we specify further commands.
  
- **Adding an employee**:
  ```bash
  dotnet run -- set-employee --employeeId 5 --employeeName Steve --employeeSalary 1234
  ```
- **Receiving an employee**:
  ```bash
  dotnet run -- get-employee --employeeId 5
  ```

---

## 🎛 Available commands

| Team           | Description                                          | Parameters                                                                |
| -------------- | ---------------------------------------------------- | ------------------------------------------------------------------------- |
| `init-db`      | Initializing the database and rolling out the schema | —                                                                         |
| `set-employee` | Add/Update Employee                                  | `--employeeId` (int)`--employeeName` (string)`--employeeSalary` (decimal) |
| `get-employee` | Getting employee data by ID                          | `--employeeId` (int)                                                      |

---

## ✅ Выполнено

- Booting up a PostgreSQL Docker container and rolling out the schema via `setUpDatabase.ps1`
- Console application on **.NET 6** with **Entity Framework Core** and **Npgsql**
- `` and `` commands via **System.CommandLine**
- Command `` to initialize the database from the application
- Automatic verification via `verifySubmission.ps1`

