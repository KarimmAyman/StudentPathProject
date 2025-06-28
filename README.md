
# StudentPath (Aoun Platform) 

**StudentPath** is a comprehensive RESTful API platform built with ASP.NET Core 8.0, designed specifically for university students in Egypt. As part of the larger Aoun ecosystem, it provides integrated solutions for transportation (ride-sharing), housing management, job opportunities, and community activities through a secure, scalable backend system. 

##  Project Overview

StudentPath serves as the backbone API for student life support services, offering a multi-domain platform that addresses the core needs of university students. The system implements role-based access control, secure payment processing, and real-time service management through a well-architected layered approach.

##  Key Features

###  Authentication & User Management
- **Role-based access control** using ASP.NET Core Identity 
- **JWT-based authentication** with secure token management 
- **Email verification and OTP workflows** for password reset 
- Multi-user type support (Students, Drivers, Admins)

###  Transportation Module
- **Driver onboarding and vehicle registration** with document verification 
- **Trip creation and management** with real-time seat tracking 
- **Automated trip status management** via background services 
- AI-powered face verification for driver approval 

###  Payment Integration
- **Dual payment gateway support** (Stripe & Paymob) for local and international transactions 
- **Escrow and wallet transaction support** for secure payments  
- Comprehensive transaction tracking and history

###  Housing Management
- **Property listing with location metadata** and geographic search  
- Multi-image support for property showcasing
- Owner-managed property posting system

###  Job & Activity Management
- **Admin-posted job and activity opportunities** 
- **Public listings with expiry management** 
- Detailed job descriptions and application tracking

##  Tech Stack & Tools

| Technology | Version | Purpose |
|------------|---------|---------|
| **ASP.NET Core** | 8.0 | Web API Framework  |
| **Entity Framework Core** | 8.0 | ORM + SQL Server Integration  |
| **ASP.NET Identity** | 8.0 | Authentication & Authorization   |
| **JWT Bearer** | 8.0 | Token-based Authentication   |
| **AutoMapper** | 13.0.1 | Object-Object Mapping   |
| **Stripe.NET** | 48.0.2 | International Payment Processing   |
| **MailKit** | 4.8.0 | Email Services   |
| **Swagger/OpenAPI** | 6.4.0 | API Documentation   |
| **SQL Server** | - | Primary Database |
| **Paymob API** | - | Egyptian Payment Gateway |

##  Installation & Setup Guide

### Prerequisites
- **.NET 8.0 SDK** or later
- **SQL Server** (LocalDB, Express, or Full)
- **Visual Studio 2022** or **VS Code** with C# extension
- **Git** for version control

### Step-by-Step Setup

1. **Clone the Repository**
   ```bash
   git clone https://github.com/KarimmAyman/StudentPathProject.git
   cd StudentPathProject
   ```

2. **Configure Database Connection**
   
   Update `appsettings.json` with your SQL Server connection string: 
   
   ```json
   {
     "ConnectionStrings": {
       "cs": "Server=(localdb)\\mssqllocaldb;Database=StudentPathDB;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Configure JWT Settings**
   
   Add JWT configuration to `appsettings.json`:
   ```json
   {
     "JWT": {
       "SecretKey": "your-super-secret-key-minimum-32-characters",
       "Issuer": "StudentPathAPI",
       "Audience": "StudentPathClients"
     }
   }
   ```

4. **Configure Payment Gateways**
   
   Add Stripe and Paymob settings:
   ```json
   {
     "Stripe": {
       "SecretKey": "sk_test_your_stripe_secret_key",
       "PublishableKey": "pk_test_your_stripe_publishable_key"
     },
     "Paymob": {
       "ApiKey": "your_paymob_api_key",
       "IntegrationId": "your_integration_id"
     }
   }
   ```

5. **Install Dependencies**
   ```bash
   dotnet restore
   ```

6. **Run Entity Framework Migrations**
   ```bash
   dotnet ef database update --project StudentPath
   ```

7. **Build and Run the Application**
   ```bash
   dotnet run --project StudentPath
   ```

The API will be available at:
- **HTTPS**: `https://localhost:7000`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:7000/swagger`

##  API Usage & Authentication Flow

### Authentication Process

1. **User Registration**
   ```http
   POST /api/accounts/Register
   Content-Type: multipart/form-data
   ```  

2. **User Login**
   ```http
   POST /api/accounts/Login
   Content-Type: application/json
   
   {
     "email": "user@example.com",
     "password": "Password123!",
     "rememberMe": false
   }
   ``` 

3. **Using JWT Token**
   ```http
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

### Key API Endpoints

| Endpoint Category | Base Route | Description |
|------------------|------------|-------------|
| **Authentication** | `/api/accounts/*` | Registration, login, password reset |
| **User Management** | `/api/user/*` | User CRUD operations   |
| **Transportation** | `/api/trips/*` | Trip management and booking |
| **Driver Services** | `/api/driver/*` | Driver onboarding and management |
| **Housing** | `/api/property/*` | Property listings and management |
| **Admin Panel** | `/api/admin/*` | Administrative operations  |

### Testing with Swagger

1. Navigate to `https://localhost:7000/swagger`
2. Click **"Authorize"** button [27](#1-26) 
3. Enter: `Bearer your-jwt-token-here`
4. Test endpoints with built-in Swagger UI

##  Code Structure & Architecture

The project follows a **layered architecture pattern** with clear separation of concerns: 

```
StudentPath/
├── StudentPath.API/                  #  Presentation Layer
│   ├── Controllers/                  # API Controllers
│   │   ├── AccountsController.cs     # Authentication endpoints
│   │   ├── UserController.cs         # User management
│   │   ├── DriverController.cs       # Driver services
│   │   ├── TripController.cs         # Transportation
│   │   ├── PropertyController.cs     # Housing management
│   │   └── AdminController.cs        # Admin operations
│   ├── Program.cs                    # Application bootstrap
│   └── appsettings.json             # Configuration
│
├── StudentPath.BLL/                  #  Business Logic Layer
│   ├── Services/                     # Service implementations
│   │   ├── AccountService/           # Authentication logic
│   │   ├── UserServices/             # User management
│   │   ├── DriverServices/           # Driver operations
│   │   ├── TripServices/             # Transportation logic
│   │   └── HousingServices/          # Property management
│   ├── DTOs/                         # Data Transfer Objects
│   └── AutoMappers/                  # Object mapping profiles
│
├── StudentPath.DAL/                  #  Data Access Layer
│   ├── Data/
│   │   ├── Models/                   # Entity models
│   │   │   ├── User.cs              # User entities
│   │   │   ├── Trip.cs              # Transportation models
│   │   │   ├── Payment.cs           # Financial models
│   │   │   └── Property.cs          # Housing models
│   │   └── DBHelpers/
│   │       └── StudentPathContext.cs # EF Core DbContext
│   └── Repositories/                 # Repository pattern
│       ├── UnitOfWork/              # Unit of Work implementation
│       └── Interfaces/              # Repository contracts
```

### Architecture Principles

- **Dependency Injection**: Comprehensive DI container setup 
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction coordination
- **Service Layer**: Business logic encapsulation
- **DTO Pattern**: Data transfer optimization

##  Contributing Guidelines

We welcome contributions to StudentPath! Please follow these guidelines:

### Development Workflow

1. **Fork the repository** and create a feature branch
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Follow coding standards**
   - Use meaningful variable and method names
   - Add XML documentation for public APIs
   - Follow the existing layered architecture pattern
   - Maintain the established project structure 

3. **Write comprehensive tests**
   - Unit tests for business logic
   - Integration tests for API endpoints
   - Mock external dependencies

4. **Update documentation**
   - Update README if adding new features
   - Add/update API documentation
   - Include code comments for complex logic

5. **Submit a Pull Request**
   - Provide clear description of changes
   - Reference related issues
   - Ensure all tests pass

### Code Review Process

- All PRs require review from maintainers
- Automated CI/CD checks must pass
- Follow semantic versioning for releases

##  License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.


##  Contact & Support

### Project Maintainers
- **Mohamed Saber** – .NET Backend Developer  
  GitHub: [@DevMohamedd](https://github.com/DevMohamedd)

- **Gamal Elbatawy** – .NET Backend Developer  
  GitHub: [@gamalgithue](https://github.com/gamalgithue)

- **Karim Ayman** – .NET Backend Developer  
  GitHub: [@KarimmAyman](https://github.com/KarimmAyman)

### Getting Help
- **Issues**: [GitHub Issues](https://github.com/KarimmAyman/StudentPathProject/issues)
- **Discussions**: [GitHub Discussions](https://github.com/KarimmAyman/StudentPathProject/discussions)


### Community
- **Discord**: [StudentPath Community](https://discord.gg/studentpath)
- **LinkedIn**: [Aoun Platform](https://linkedin.com/company/aoun-platform)

---

**If you find this project helpful, please consider giving it a star on GitHub!**

**Built with ❤️ By Aoun Team**

---

##  Contributors

| Name              | Role                  | GitHub Profile                                      |
|-------------------|------------------------|-----------------------------------------------------|
| Mohamed Saber     | .NET Backend Developer | [@DevMohamedd](https://github.com/DevMohamedd)     |
| Gamal Elbatawy    | .NET Backend Developer | [@gamalgithue](https://github.com/gamalgithue)     |
| Karim Ayman       | .NET Backend Developer | [@KarimmAyman](https://github.com/KarimmAyman)     |


