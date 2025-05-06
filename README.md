# 🧩 Candidate Hub API

A scalable, testable, and well-structured **RESTful Web API** built with **ASP.NET Core** for managing job candidate data. This project is designed to simulate a real-world backend system used in HR platforms and supports key enterprise patterns like **Unit of Work**, **Repository**, **Caching**, and **DTO mapping**.

---------

## 🚀 Features

- ✅ Add or update candidate by email
- ✅ Fetch all candidates or by email
- ✅ SQL-based persistence with **EF Core Code-First**
- ✅ Dependency Injection & Unit of Work pattern
- ✅ Caching with `IMemoryCache`
- ✅ Auto migration on startup
- ✅ Strong validation using data annotations
- ✅ Clean architecture (API → App → Domain → Infrastructure)
- ✅ Unit & integration tests using `xUnit`, `Moq`, and `InMemory`

---------

## 📁 Project Structure

```bash
CandidateHub/
├── CandidateHub.API              # Web API Layer
├── CandidateHub.Application      # Business Logic Layer
├── CandidateHub.Domain           # Entities and Interfaces
├── CandidateHub.Infrastructure   # Data Access, Services
├── CandidateHub.Tests            # Unit and Integration Tests
```

---------

## 🛠️ Technologies Used

- ASP.NET Core 8
- EF Core (Code-First + Migrations)
- SQL Server / LocalDB
- xUnit + Moq + InMemory Testing
- IMemoryCache
- Riok.Mapperly for DTO mapping
- Swagger

---------

## ⚙️ Getting Started

### 🔑 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- Visual Studio or VS Code

---------

### 🚦 Setup Instructions

1. **Clone the repository**

```bash
git clone https://github.com/AssimQader/CandidateHubAPI.git
cd candidate-hub-api
```

2. **Update Database with Migrations**

You can do this in **two ways**:

✅ **Option A – Run automatically on startup**  
Just run the app — migrations will be applied automatically.

✅ **Option B – Manually via CLI**

```bash
cd CandidateHub.API
dotnet ef database update
```

3. **Run the API**

```bash
dotnet run --project CandidateHub.API
```

4. **Access Swagger UI**

--------

## 📌 Sample Payload

### POST `/api/candidates`

```json
{
  "firstName": "example_firstname",
  "lastName": "example_lastname",
  "email": "example@example.com",
  "phoneNumber": "+201234567890",
  "callTimePreference": 3,
  "linkedInUrl": "https://linkedin.com/example",
  "gitHubUrl": "https://github.com/example",
  "comments": "......."
}
```

---------

## 🧪 Running Tests

### Unit Tests

```bash
dotnet test CandidateHub.Tests
```

Tests include:
- Service logic
- Controller integration
- Validation
- Caching behavior

---------

## 📦 Design Patterns Used

| Pattern           | Purpose                                  |
|-------------------|-------------------------------------------|
| Repository        | Abstracts data access                     |
| Unit of Work      | Manages atomic transactions               |
| DTO + Mapperly    | Clean separation of domain + API models   |

---------

## 🔒 Validation Rules

All fields are validated using data annotations:

| Field         | Validation                             |
|---------------|-----------------------------------------|
| First Name    | Required, max 100 chars                 |
| Last Name     | Required, max 100 chars                 |
| Email         | Required, unique, email format          |
| Phone Number  | Required, max 20, E.164 format only     |
| LinkedIn URL  | Optional, max 255                       |
| GitHub URL    | Optional, max 255                       |
| Comments      | Optional, max 2000                      |

---------

## 🙌 For Contributing

1. Fork the repo
2. Create a branch: `git checkout -b feature/your-feature`
3. Commit changes: `git commit -m "Add feature"`
4. Push: `git push origin feature/your-feature`
5. Open a Pull Request

---------

## 👨‍💻 Author

Developed by **Asem Adel**  
For any queries, feel free to [contact me](mailto:asem.adel00@gmail.com) or raise an issue in the repository.

---------
