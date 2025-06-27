# Aoun 🎓🛏️🚌💼

**Aoun** is a full-stack, multi-domain service platform built for university students, offering integrated solutions for **transportation**, **housing**, **job opportunities**, and **community activities**. Developed using ASP.NET Core 8.0, it provides a secure, scalable RESTful API for mobile and web applications.

---

## 📌 Features

### ✅ User Management & Authentication
- Role-based access control using ASP.NET Identity
- Email verification and OTP workflows
- JWT-based authentication

### 🚗 Transportation Module
- Driver onboarding and vehicle registration
- Trip creation and management
- Seat booking with status tracking
- TripStatus background service automation

### 💰 Payment Integration
- Dual-gateway support: **Stripe** & **Paymob**
- Escrow and wallet transaction support
- Secure payment and transaction tracking

### 🏠 Housing Management
- Property listing with location metadata
- Multi-image support and search by location
- Owner-managed property posting

### 💼 Job & Activity Management
- Admin-posted job and activity opportunities
- Publicly accessible listings
- Expiry date and detailed descriptions

---

## 🧠 Architecture Overview

### 🏗️ Layered Architecture
- **API Layer**: Controllers & Endpoints
- **Business Logic Layer (BLL)**: Service interfaces and implementations
- **Data Access Layer (DAL)**: EF Core repositories, Unit of Work pattern

### 🔄 Domain Services
- `AccountService`, `DriverService`, `TripService`, `PropertyService`, `JobService`
- Background service: `TripStatusBackgroundService`

### 🔐 Security
- JWT Bearer Token Authentication
- ASP.NET Core Identity for user and role management
- CORS policy (`AllowFrontend`) for frontend integration

---

## 🔧 Technologies

| Technology         | Description                            |
|--------------------|----------------------------------------|
| ASP.NET Core 8.0   | API Framework                          |
| Entity Framework   | ORM + SQL Server                       |
| ASP.NET Identity   | User/Roles/Auth                        |
| AutoMapper         | DTO mapping                            |
| Stripe.NET         | Credit card payments                   |
| Paymob API         | Egyptian payment gateway               |
| MailKit            | Email services via Gmail SMTP          |
| Swashbuckle        | Swagger UI for API documentation       |

---

## ⚙️ Project Structure

```text
StudentPath/
├── StudentPath.API/                  # Main API project
│   └── Controllers/
│       ├── AccountsController.cs
│       ├── DriverController.cs
│       ├── TripController.cs
│       └── PropertyController.cs
│
├── StudentPath.BLL/                 # Business logic layer
│   └── Services/
│       ├── AccountService.cs
│       ├── DriverService.cs
│       └── TripService.cs
│
├── StudentPath.DAL/                 # Data access layer
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Driver.cs
│   │   ├── Payment.cs
│   │   └── Trip.cs
│   └── Repositories/
│       ├── PropertyRepository.cs
│       └── JobRepository.cs
│
├── StudentPathContext.cs            # EF Core DB context
├── UnitOfWork.cs                    # Coordinated transaction handler
├── appsettings.json                 # Configuration (DB, JWT, Stripe, etc.)
└── Program.cs                       # App startup, service registration
