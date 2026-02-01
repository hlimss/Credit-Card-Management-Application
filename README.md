# 💳 Credit Card Management Application

> A comprehensive, production-ready full-stack web application for managing credit cards with enterprise-grade security, modern UI, and advanced banking features.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Vue.js](https://img.shields.io/badge/Vue.js-3.3-4FC08D?logo=vue.js)](https://vuejs.org/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/License-Evaluation-999999)](LICENSE)

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Technology Stack](#-technology-stack)
- [Architecture](#-architecture)
- [Prerequisites](#-prerequisites)
- [Quick Start](#-quick-start)
- [Project Structure](#-project-structure)
- [API Documentation](#-api-documentation)
- [Security](#-security)
- [Database Schema](#-database-schema)
- [Evaluation Criteria](#-evaluation-criteria)
- [Contributing](#-contributing)

## 🎯 Overview

This application provides a complete solution for credit card management with:

- **Secure Authentication**: JWT-based authentication with OAuth 2.0 support (Google, Facebook)
- **Complete CRUD Operations**: Full Create, Read, Update, Delete functionality for credit cards
- **Advanced Banking Features**: Transfers, payments, statements, loans, and analytics
- **User Data Isolation**: Strict user-based access control ensuring data privacy
- **Rigorous Validation**: Luhn algorithm for card numbers, expiration date validation, and comprehensive input sanitization
- **Modern Governmental UI/UX**: Professional institutional design with responsive layout, sidebar navigation, and real-time notifications
- **Transaction Security**: Confirmation code system for secure card transactions
- **Card Management**: Activate/deactivate cards, profile management, and comprehensive transaction tracking

## ✨ Features

### 🔐 Authentication & Authorization
- ✅ **JWT Authentication**: Secure token-based authentication system
- ✅ **OAuth 2.0 Integration**: Google and Facebook login support
- ✅ **User Registration**: Email validation and secure password hashing (BCrypt)
- ✅ **User Login/Logout**: Complete authentication flow
- ✅ **Protected Routes**: Frontend and backend route guards
- ✅ **Session Management**: Secure cookie-based session handling

### 💳 Credit Card Management (CRUD)
- ✅ **Create**: Add new credit cards with comprehensive validation and confirmation codes
- ✅ **Read**: View all user's credit cards with 3D visualization and status indicators
- ✅ **Update**: Edit existing credit card information, including activation status
- ✅ **Delete**: Remove credit cards with confirmation
- ✅ **User Isolation**: Users can only access their own cards (enforced at API level)
- ✅ **Card Categories**: Organize cards by category (Personnel, Travail, Voyage, etc.)
- ✅ **Balance Tracking**: Monitor card balances and limits with automatic updates
- ✅ **Card Activation**: Activate/deactivate cards to control transaction permissions
- ✅ **Confirmation Codes**: Secure transaction confirmation system per card
- ✅ **Expiration Alerts**: Automatic notifications for expiring cards

### 💸 Banking Features
- ✅ **Bank Transfers**: Internal, external, and international transfers with balance updates
- ✅ **Payments**: Vignettes, subscriptions, bills, and more with real-time notifications
- ✅ **Statements**: Generate detailed card statements with transaction history
- ✅ **Loans**: Create and manage loans with payment tracking and calculations
- ✅ **Analytics**: Comprehensive statistics, trends, and expense analysis with charts
- ✅ **Transactions**: Dedicated transactions page with filtering, creation, and comprehensive tracking
- ✅ **Transaction Security**: Confirmation code required for all transactions on protected cards
- ✅ **Automatic Balance Updates**: Real-time balance deduction/addition based on transaction type

### 🔒 Security & Validation
- ✅ **Card Number Validation**: Luhn algorithm implementation
- ✅ **Expiration Date Validation**: Future date enforcement (MM/YY format)
- ✅ **CVV Validation**: 3-4 digit validation based on card type
- ✅ **Data Encryption**: Credit card numbers and CVV encrypted at rest
- ✅ **Input Sanitization**: Server-side and client-side validation
- ✅ **SQL Injection Prevention**: Entity Framework Core parameterized queries
- ✅ **CORS Configuration**: Properly configured for secure cross-origin requests

### 🎨 User Interface
- ✅ **Governmental Design**: Professional institutional interface inspired by modern government portals
- ✅ **Responsive Design**: Mobile-first approach with sidebar navigation and bottom navigation on mobile
- ✅ **Institutional Sidebar**: Retractable sidebar with quick access to all features
- ✅ **Live Currency Rates**: Real-time exchange rates ticker in header
- ✅ **Notification System**: Global notification system with unread count and dropdown
- ✅ **3D Card Visualization**: Holographic card effects with animations and status indicators
- ✅ **Real-time Validation**: Instant feedback on form inputs with helpful error messages
- ✅ **Loading States**: Visual feedback during API calls
- ✅ **Error Handling**: User-friendly error messages with context-specific tips
- ✅ **Dashboard**: Bank of Africa-inspired dashboard with KPIs, quick actions, and analytics
- ✅ **Dedicated Pages**: Separate pages for cards, transactions, settings, and analytics
- ✅ **Professional Footer**: Institutional footer with links and information
- ✅ **Settings Page**: User profile management and card activation controls

## 🛠️ Technology Stack

### Backend
- **ASP.NET Core 9.0** - Web API framework
- **Entity Framework Core 9.0** - ORM for database operations
- **SQL Server** - Relational database management system
- **JWT Bearer Authentication** - Token-based authentication
- **BCrypt.NET** - Password hashing
- **Swagger/OpenAPI** - API documentation
- **FluentValidation** - Input validation

### Frontend
- **Vue.js 3** - Progressive JavaScript framework (Composition API)
- **Pinia** - State management library (auth, credit cards, notifications)
- **Vue Router 4** - Client-side routing with protected routes
- **Axios** - HTTP client for API communication with interceptors
- **Vite** - Next-generation frontend build tool
- **Tailwind CSS** - Utility-first CSS framework with custom governmental theme
- **Chart.js** - Data visualization (for analytics)
- **Heroicons/Lucide** - Icon library for UI components

## 🏗️ Architecture

### Backend Architecture
```
CreditCardManagement.API/
├── Controllers/          # API endpoints
│   ├── AuthController.cs
│   ├── CreditCardsController.cs
│   ├── TransactionsController.cs
│   ├── BankTransfersController.cs
│   ├── PaymentsController.cs
│   ├── StatementsController.cs
│   └── LoansController.cs
├── Services/             # Business logic layer
│   ├── AuthService.cs
│   ├── CreditCardService.cs
│   ├── TransactionService.cs
│   └── ...
├── Models/               # Entity models
│   ├── User.cs
│   ├── CreditCard.cs
│   ├── Transaction.cs
│   └── ...
├── DTOs/                 # Data Transfer Objects
├── Data/                 # Database context
│   └── ApplicationDbContext.cs
└── Program.cs           # Application entry point
```

### Frontend Architecture
```
CreditCardManagement.Frontend/
├── src/
│   ├── components/       # Reusable Vue components
│   │   ├── CreditCard3D.vue
│   │   ├── BankSidebar.vue
│   │   ├── TransferModal.vue
│   │   └── ...
│   ├── views/           # Page components
│   │   ├── Home.vue
│   │   ├── Login.vue
│   │   └── ...
│   ├── stores/          # Pinia state management
│   │   ├── auth.js
│   │   └── creditCard.js
│   ├── services/        # API service layer
│   │   ├── api.js
│   │   ├── authService.js
│   │   └── ...
│   └── router/          # Vue Router configuration
└── public/              # Static assets
```

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 9.0 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- **Node.js 18+** and npm ([Download](https://nodejs.org/))
- **SQL Server** (LocalDB, Express, or full version) ([Download](https://www.microsoft.com/sql-server/sql-server-downloads))
- **Git** for version control ([Download](https://git-scm.com/))

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/credit-card-management.git
cd credit-card-management
```

### 2. Backend Setup

```bash
cd CreditCardManagement.API

# Copy example configuration
cp appsettings.example.json appsettings.json
cp appsettings.Development.example.json appsettings.Development.json

# Edit appsettings.json with your database connection string
# Edit appsettings.json with your OAuth credentials (optional)

# Run the application
dotnet run --urls "http://localhost:5000"
```

The API will be available at `http://localhost:5000`  
Swagger documentation: `http://localhost:5000/swagger`

### 3. Frontend Setup

```bash
cd CreditCardManagement.Frontend

# Install dependencies
npm install

# Copy environment example
cp env.example .env

# Run development server
npm run dev
```

The frontend will be available at `http://localhost:5173`

### 4. Database Setup

The database is created automatically on first run. If you need to create it manually:

```bash
# Using Entity Framework Core tools
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 📁 Project Structure

```
Credit Card Management Application/
├── CreditCardManagement.API/          # Backend API
│   ├── Controllers/                   # API Controllers
│   ├── Services/                      # Business Logic
│   ├── Models/                        # Entity Models
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Data/                          # Database Context
│   └── Program.cs                     # Application Entry
├── CreditCardManagement.Frontend/     # Frontend Application
│   ├── src/
│   │   ├── components/                # Vue Components
│   │   ├── views/                     # Page Views
│   │   ├── stores/                    # Pinia Stores
│   │   ├── services/                  # API Services
│   │   └── router/                    # Vue Router
│   └── public/                        # Static Assets
├── README.md                          # This file
├── SETUP.md                           # Detailed setup guide
└── TEST_CARD_NUMBERS.md               # Test card numbers
```

## 📝 API Documentation

### Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login` | Login user |
| `POST` | `/api/auth/logout` | Logout user |
| `GET` | `/api/auth/me` | Get current user details |
| `PUT` | `/api/auth/me` | Update user profile information |
| `GET` | `/api/oauth/google` | Initiate Google OAuth |
| `GET` | `/api/oauth/facebook` | Initiate Facebook OAuth |

### Credit Card Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/creditcards` | Get all user's credit cards |
| `GET` | `/api/creditcards/{id}` | Get specific credit card |
| `POST` | `/api/creditcards` | Create new credit card |
| `PUT` | `/api/creditcards/{id}` | Update credit card |
| `DELETE` | `/api/creditcards/{id}` | Delete credit card |

### Transaction Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/transactions` | Get all user's transactions |
| `GET` | `/api/transactions/{id}` | Get specific transaction |
| `POST` | `/api/transactions` | Create new transaction (requires confirmation code if card has one) |
| `GET` | `/api/transactions/analytics` | Get transaction analytics and statistics |
| `PUT` | `/api/transactions/{id}` | Update transaction |
| `DELETE` | `/api/transactions/{id}` | Delete transaction |

### Banking Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/banktransfers` | Create bank transfer |
| `GET` | `/api/banktransfers` | Get all transfers |
| `POST` | `/api/payments` | Create payment |
| `GET` | `/api/payments` | Get all payments |
| `POST` | `/api/statements` | Generate statement |
| `GET` | `/api/statements` | Get all statements |
| `POST` | `/api/loans` | Create loan |
| `GET` | `/api/loans` | Get all loans |

**Full API Documentation**: Available at `/swagger` when the backend is running.

## 🔒 Security

### Authentication
- JWT tokens with configurable expiration
- Secure password hashing using BCrypt
- OAuth 2.0 integration (Google, Facebook)
- Token validation on every protected request

### Authorization
- User-based data access control
- Protected API endpoints with `[Authorize]` attribute
- Frontend route guards for authenticated pages
- Database-level user isolation

### Data Protection
- Sensitive data encryption (card numbers, CVV)
- Data masking in API responses
- Never expose CVV in responses
- Secure cookie configuration for OAuth

### Input Validation
- Server-side validation (required)
- Client-side validation (UX enhancement)
- SQL injection prevention (EF Core)
- XSS protection
- CORS configuration

## 📊 Database Schema

### Core Entities

**Users**
- `Id` (Guid, PK)
- `Email` (string, unique)
- `PasswordHash` (string)
- `FirstName`, `LastName` (string)
- `Provider`, `ProviderId` (OAuth)
- `CreatedAt` (DateTime)

**CreditCards**
- `Id` (Guid, PK)
- `UserId` (Guid, FK → Users)
- `CardNumber` (string, encrypted)
- `CardholderName` (string)
- `ExpirationDate` (string, MM/YY)
- `CVV` (string, encrypted)
- `CardType` (string)
- `Balance` (decimal)
- `Category` (string)
- `IsActive` (bool) - Card activation status
- `ConfirmationCode` (string, nullable) - Transaction confirmation code
- `CreatedAt`, `UpdatedAt` (DateTime)

**Transactions**
- `Id` (Guid, PK)
- `CreditCardId` (Guid, FK → CreditCards)
- `MerchantName` (string)
- `Amount` (decimal)
- `Currency` (string)
- `Category` (string)
- `TransactionType` (string: Expense, Credit, Refund)
- `TransactionDate` (DateTime)
- `ConfirmationCode` (string, nullable) - Confirmation code used for transaction
- `Location` (string, nullable)
- `Description` (string, nullable)

**BankTransfers**
- `Id` (Guid, PK)
- `UserId` (Guid, FK → Users)
- `FromAccount`, `ToAccount` (string)
- `BeneficiaryName` (string)
- `Amount` (decimal)
- `TransferType` (string)
- `Status` (string)

**Payments**
- `Id` (Guid, PK)
- `UserId` (Guid, FK → Users)
- `CreditCardId` (Guid, FK → CreditCards, nullable)
- `PaymentType` (string)
- `MerchantName` (string)
- `Amount` (decimal)
- `Status` (string)

**Statements**
- `Id` (Guid, PK)
- `UserId` (Guid, FK → Users)
- `CreditCardId` (Guid, FK → CreditCards)
- `StatementType` (string)
- `OpeningBalance`, `ClosingBalance` (decimal)
- `TotalCredits`, `TotalDebits` (decimal)
- `TransactionCount` (int)

**Loans**
- `Id` (Guid, PK)
- `UserId` (Guid, FK → Users)
- `LoanType` (string)
- `PrincipalAmount`, `RemainingAmount` (decimal)
- `InterestRate` (decimal)
- `MonthlyPayment` (decimal)
- `Status` (string)

## ✅ Evaluation Criteria

### Code Quality and Architecture ✅
- ✅ Clean code principles and SOLID patterns
- ✅ Separation of concerns (Controllers, Services, Data layers)
- ✅ Proper error handling and logging
- ✅ Well-documented code with XML comments
- ✅ Consistent naming conventions
- ✅ Dependency injection throughout

### Security and Best Practices ✅
- ✅ JWT authentication with secure token management
- ✅ Password hashing with BCrypt
- ✅ Data encryption for sensitive information
- ✅ Input validation (server-side and client-side)
- ✅ User data isolation enforced at multiple levels
- ✅ CORS configuration
- ✅ SQL injection prevention
- ✅ XSS protection

### User Experience ✅
- ✅ Responsive design (mobile, tablet, desktop)
- ✅ Intuitive and modern user interface
- ✅ Real-time validation feedback
- ✅ Loading states and error handling
- ✅ 3D card visualization
- ✅ Smooth animations and transitions
- ✅ Professional appearance

## 🧪 Testing

### Test Card Numbers

Valid test card numbers for development (see `TEST_CARD_NUMBERS.md`):
- **Visa**: `4111 1111 1111 1111`
- **MasterCard**: `5555 5555 5555 4444`
- **Amex**: `3782 822463 10005`

### Running Tests

```bash
# Backend tests
cd CreditCardManagement.API
dotnet test

# Frontend tests
cd CreditCardManagement.Frontend
npm run test
```

## 📦 Building for Production

### Backend

```bash
cd CreditCardManagement.API
dotnet publish -c Release -o ./publish
```

### Frontend

```bash
cd CreditCardManagement.Frontend
npm run build
```

The production build will be in the `dist/` directory.

## 🔧 Configuration

### Environment Variables

**Backend** (`appsettings.json`):
- `ConnectionStrings:DefaultConnection` - SQL Server connection string
- `JwtSettings:SecretKey` - JWT signing key (change in production!)
- `OAuth:Google:ClientId` - Google OAuth client ID
- `OAuth:Google:ClientSecret` - Google OAuth client secret
- `OAuth:Facebook:AppId` - Facebook app ID
- `OAuth:Facebook:AppSecret` - Facebook app secret

**Frontend** (`.env`):
- `VITE_API_BASE_URL` - Backend API URL (default: `http://localhost:5000`)

## 📚 Additional Documentation

- **SETUP.md** - Detailed setup instructions
- **TEST_CARD_NUMBERS.md** - Valid test card numbers for development
- **Swagger UI** - Interactive API documentation at `/swagger`

## 🎓 Learning Outcomes

This project demonstrates:

- Full-stack development (Backend + Frontend)
- RESTful API design and implementation
- Authentication & Authorization patterns
- Database design and ORM usage (Entity Framework Core)
- Modern frontend frameworks (Vue.js 3)
- State management (Pinia)
- Security best practices
- Code organization and clean architecture
- User experience design
- Responsive web design

## 🆕 Recent Updates (v2.0.0)

### New Features
- ✅ **Governmental/Institutional UI Design**: Complete redesign with professional government portal aesthetics
- ✅ **Notification System**: Global notification system with unread count, dropdown, and persistence
- ✅ **Transaction Confirmation Codes**: Secure transaction system requiring confirmation codes per card
- ✅ **Card Activation/Deactivation**: Control card usage with activation toggle in settings
- ✅ **Dedicated Transactions Page**: Comprehensive transaction management with filtering and creation
- ✅ **Settings Page**: User profile management and card activation controls
- ✅ **Live Currency Rates**: Real-time exchange rates ticker in header
- ✅ **Institutional Sidebar**: Retractable sidebar with all banking features
- ✅ **Professional Footer**: Institutional footer with links and information
- ✅ **Enhanced Error Handling**: Improved error messages with context-specific tips
- ✅ **Automatic Balance Updates**: Real-time balance deduction/addition based on transaction type
- ✅ **Transaction Blocking**: Inactive cards cannot be used for transactions

### UI/UX Improvements
- Modern governmental color palette (navy blue, slate gray, off-white)
- Responsive sidebar navigation (desktop: retractable, mobile: bottom navigation)
- Professional header with live rates and notifications
- Bank of Africa-inspired dashboard design
- Improved card visualization with status indicators
- Enhanced form validation with helpful error messages

## 📅 Project Information

- **Deadline**: February 2nd, 2026
- **Status**: ✅ Complete and Production-Ready
- **Version**: 2.0.0

## 👨‍💻 Development

### Code Standards
- C# coding conventions
- Vue.js style guide compliance
- RESTful API design principles
- Clean architecture patterns

### Git Workflow
- Feature branches
- Descriptive commit messages
- Code reviews

## 📄 License

This project is created for evaluation purposes.

## 🙏 Acknowledgments

- Vue.js team for the excellent framework
- ASP.NET Core team for the robust backend framework
- All open-source contributors whose libraries made this project possible

---

**Built with ❤️ using ASP.NET Core and Vue.js 3**
