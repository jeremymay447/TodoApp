# Todo API - .NET Backend

A RESTful API for the Todo application built with ASP.NET Core 8 and Entity Framework Core with SQLite. Created using Claude.

## Features

- ✅ Full CRUD operations for todos
- 🗄️ SQLite database with Entity Framework Core
- 📝 Data validation with DTOs
- 🔄 CORS enabled for React frontend
- 📚 Swagger/OpenAPI documentation
- 🪵 Structured logging

## Tech Stack

- **ASP.NET Core 8** - Web API framework
- **Entity Framework Core 8** - ORM
- **SQLite** - Lightweight database
- **Swagger** - API documentation

## API Endpoints

### Get all todos
```
GET /api/todos
```

### Get todo by ID
```
GET /api/todos/{id}
```

### Create todo
```
POST /api/todos
Content-Type: application/json

{
  "title": "Buy groceries",
  "description": "Milk, eggs, bread"
}
```

### Update todo
```
PUT /api/todos/{id}
Content-Type: application/json

{
  "title": "Buy groceries",
  "description": "Milk, eggs, bread, butter",
  "completed": false
}
```

### Toggle todo completion
```
PATCH /api/todos/{id}/toggle
```

### Delete todo
```
DELETE /api/todos/{id}
```

## Getting Started

### Prerequisites

- .NET 8 SDK installed ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))

### Installation

1. Restore dependencies:
```bash
dotnet restore
```

2. Run the application:
```bash
dotnet run
```

The API will start at `https://localhost:5001` (or `http://localhost:5000`)

### View API Documentation

Once running, navigate to:
- Swagger UI: `https://localhost:5001/swagger`

### Database

The SQLite database (`todos.db`) is created automatically on first run. No migration commands needed!

## Project Structure

```
TodoApi/
├── Controllers/
│   └── TodosController.cs      # REST API endpoints
├── Data/
│   └── TodoContext.cs          # EF Core DbContext
├── DTOs/
│   └── TodoDtos.cs             # Request/Response models
├── Models/
│   └── Todo.cs                 # Entity model
├── Program.cs                  # App configuration
├── appsettings.json            # Configuration
└── TodoApi.csproj              # Project file
```

## Development

### Run in watch mode (auto-restart on changes)
```bash
dotnet watch run
```

### Build for production
```bash
dotnet publish -c Release -o ./publish
```

## Connecting to React Frontend

1. Make sure the API is running on port 5000 or 5001
2. Update the React frontend API URL to point to `http://localhost:5000/api` or `https://localhost:5001/api`
3. CORS is already configured to allow requests from `http://localhost:3000` and `http://localhost:5173`

## Environment Variables

You can override settings using environment variables or `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=todos.db"
  }
}
```

## License

MIT
