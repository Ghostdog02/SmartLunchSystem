# 🌐 DevGuard is a Cross-Platform MVC Web Application with Google Authentication & Role-Based Access

## 🚀 Overview

This project is a robust, cross-platform ASP.NET Core MVC web application with the following key features:

- ✅ **Google Authentication Integration**  
- ✅ **Role-Based Access Control using Microsoft Identity**  
- ✅ **API Controller for External Communication**  
- ✅ **Entity Framework with Code-First Migrations**  
- ✅ **Test-Driven Development using xUnit**  
- ✅ **Works on both Ubuntu 24.04 & Windows 11**  

Whether you're running Linux or Windows, this project is designed to be easily cloned, run, and tested locally.

---

## 📸 Features & Highlights

- 🔐 **Authentication & Authorization**
  - Login with **Google** using OAuth2
  - Microsoft Identity used for user management and **role-based authorisation**
  
- 🏗️ **MVC Architecture**
  - Separation of concerns for better maintainability, testability and scalability

- 🌐 **RESTful API Controller**
  - Backend API for data access and integration

- 💾 **Database with EF Core**
  - Code-first approach using **Entity Framework Core**
  - Seamless database migration and schema evolution

- 🧪 **Comprehensive Testing**
  - Uses **xUnit** for:
    - Unit tests for controllers and services
    - End-to-end test scenarios

- 🖥️ **Cross-Platform Support**
  - Works on both **Ubuntu 24.04** and **Windows 10/11**

---

## 🛠️ Tech Stack

- **ASP.NET Core MVC**
- **ASP.NET Core Web API (Controller API)**
- **Entity Framework Core 9**
- **Microsoft Identity**
- **Google OAuth2**
- **xUnit**
- **SQL Server 2022/SQLite**
- **C# 12+**
- **Docker**

---

## 💻 Getting Started

Follow these instructions to run the app locally:

### 1. 🧱 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) All versions of the sdk work for Windows 10/11
- [.NET 9 SDK](https://dotnet.microsoft.com/download) Install 9.0.107 for Ubuntu 24.04
- [Git](https://git-scm.com/)
- For Windows 10/11 - SQL Server (or use SQLite if configured)
- For Linux Ubuntu - [SQL Server Container with Docker](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-linux-ver17&preserve-view=true&tabs=cli&pivots=cs1-bash#pullandrun2025) (or use SQLite if configured)
- IDE - Visual Studio 2022+ or VS Code

### 2. 📦 Clone the Repository

```bash
git clone https://github.com/your-username/your-repo-name.git
cd your-repo-name
```

### 3. 🔐 Obtain OAuth 2.0 credentials from the Google API Console.
Visit the [Google API Console](https://console.developers.google.com/) to obtain OAuth 2.0 credentials such as a client ID and client secret that are known to both Google and your application.


### 4. 🔧 Configure the Web App

Update appsettings.json or secrets.json with your own:

```json
"Authentication": {
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
  }
}
```

### 5.1. 🔧 Configure the Connection String for the database for Windows 11

Update appsettings.json or secrets.json with your own (The following example uses SQL Server with Windows authentication):

```json
"ConnectionStrings": {
    "SmartLunchContextConnection": "Server=localhost;Database=SmartLunchSystem;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=true"
  }
```
### 5.2. 🔧 Configure the Connection String for the database for Linux Ubuntu 24.04
Update appsettings.json or secrets.json with your own (The following example uses SQL Server with SQL Authentication):

```json
"ConnectionStrings": {
    "SmartLunchConnection": "Server=localhost;Database=SmartLunchSystem;User Id=sa;Password=YourStrong!Passw0rd;Integrated Security=false;TrustServerCertificate=True;"
  }
```

### 6. 🛠 Add Migrations
```bash
dotnet ef migrations add "InitialCreate"
```


### 7. 🛠 Run Migrations

```bash
dotnet ef database update
```
Ensure the correct database provider is configured (e.g., SQL Server or SQLite).

### 7. ▶️ Run the Application

```bash
dotnet run
```

🔐 Roles & Permissions
You can assign roles to users via the Controller API. Examples:

Admin – Full access to all controllers and actions
SuperUser - Access to most controllers
NormalUser – Limited access based on permissions
