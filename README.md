# 🌐 Cross-Platform MVC Web Application with Google Authentication & Role-Based Access

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
  
- 🗂 **MVC Architecture**
  - Separation of concerns for better maintainability and testability

- 🌐 **RESTful API Controller**
  - Backend API for data access and integration

- 💾 **Database with EF Core**
  - Code-first approach using **Entity Framework Core**
  - Seamless database migration and schema evolution

- 🧪 **Comprehensive Testing**
  - Uses **xUnit** for:
    - Unit tests for controllers and services
    - Integration tests for database operations
    - End-to-end test scenarios

- 🖥️ **Cross-Platform Support**
  - Works on both **Ubuntu 24.04** and **Windows 11**

---

## 🛠️ Tech Stack

- **ASP.NET Core MVC**
- **Entity Framework Core**
- **Microsoft Identity**
- **Google OAuth2**
- **xUnit**
- **SQL Server / SQLite**
- **C# 12+**

---

## 💻 Getting Started

Follow these instructions to run the app locally:

### 1. 🧱 Prerequisites

- [.NET 9.0.107 SDK](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/)
- SQL Server (or use SQLite if configured)
- (Optional) Visual Studio 2022+ or VS Code

### 2. 📦 Clone the Repository

```bash
git clone https://github.com/your-username/your-repo-name.git
cd your-repo-name
```

### 3. 🔐 Obtain OAuth 2.0 credentials from the Google API Console.
Visit the Google API Console to obtain OAuth 2.0 credentials such as a client ID and client secret that are known to both Google and your application.


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

### 4. 🔧 Configure the API for Windows 11

Update appsettings.json or secrets.json with your own:

```json
"Authentication": {
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
  }
}
```
