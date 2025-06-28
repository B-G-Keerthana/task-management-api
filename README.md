# My C# Web API

Task Management API .Net project.

## Features

- User management
- Task management
- Admin and User Role-based access
- JWT authentication
- Entity Framework Core with an in-memory database
- Request Json Validations
- Role Based Middleware
- Document API endpoints using Swagger

## Getting Started

### 1. Clone the Repository

Clone this repository to your local machine and navigate to the desired directory:

```bash
cd <your-location>
```

### 2. Create and Build the Project

Run the following commands to scaffold and build the Web API project:

```bash
dotnet new webapi -n TaskManagementAPI
cd TaskManagementAPI
msbuild TaskManagementAPI.csproj
```

### 3. Restore Dependencies

Restore the required NuGet packages:

```bash
dotnet restore
```

### 4. Run the Application

Click the **Run** button at the top of Visual Studio Code, or use the terminal:

```bash
dotnet run
```

### 5. Access Swagger UI

Once the application is running, open your browser and navigate to:

```
https://localhost:7147/swagger/index.html
```

> If it doesn't open automatically, copy and paste the URL into your browser.

---
