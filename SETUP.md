# Setup Guide - Credit Card Management Application

**Version**: 2.0.0  
**Last Updated**: 2025

## Quick Start

### Prerequisites Installation

1. **Install .NET 9.0 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
   - Verify installation: `dotnet --version`

2. **Install Node.js 18+**
   - Download from: https://nodejs.org/
   - Verify installation: `node --version` and `npm --version`

3. **Install SQL Server**
   - Option 1: SQL Server LocalDB (Recommended for development)
     - Included with Visual Studio or download SQL Server Express
   - Option 2: SQL Server Express
     - Download from: https://www.microsoft.com/sql-server/sql-server-downloads

### Backend Setup

1. **Navigate to API directory**
   ```bash
   cd CreditCardManagement.API
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update connection string** (if needed)
   - Edit `appsettings.json`
   - Default uses LocalDB: `Server=(localdb)\\mssqllocaldb;Database=CreditCardDB;...`

4. **Create database**
   ```bash
   dotnet ef database update
   ```
   
   If you get an error about migrations, create them first:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the API**
   ```bash
   dotnet run
   ```
   
   The API will be available at:
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001
   - Swagger UI: http://localhost:5000/swagger

### Frontend Setup

1. **Navigate to Frontend directory**
   ```bash
   cd CreditCardManagement.Frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure API URL** (if needed)
   - Copy `env.example` to `.env`
   - Update `VITE_API_BASE_URL` if your API runs on a different port

4. **Run development server**
   ```bash
   npm run dev
   ```
   
   The frontend will be available at: http://localhost:5173

## Testing the Application

1. **Start the backend API** (Terminal 1)
   ```bash
   cd CreditCardManagement.API
   dotnet run
   ```

2. **Start the frontend** (Terminal 2)
   ```bash
   cd CreditCardManagement.Frontend
   npm run dev
   ```

3. **Open browser**
   - Navigate to: http://localhost:5173
   - Register a new account or use OAuth (Google/Facebook)
   - Login and explore the new governmental interface
   - Add credit cards with confirmation codes
   - Access settings to manage profile and activate/deactivate cards
   - Use the sidebar to access all banking features

## Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to SQL Server

**Solutions**:
1. Ensure SQL Server LocalDB is installed
2. Check connection string in `appsettings.json`
3. Try using SQL Server Express with a different connection string:
   ```
   Server=localhost\\SQLEXPRESS;Database=CreditCardDB;Trusted_Connection=True;
   ```

### CORS Errors

**Problem**: Frontend cannot connect to API

**Solutions**:
1. Ensure API is running on port 5000
2. Check CORS configuration in `Program.cs`
3. Verify `VITE_API_BASE_URL` in frontend `.env` file

### Port Already in Use

**Problem**: Port 5000 or 5173 is already in use

**Solutions**:
1. Change API port in `launchSettings.json`
2. Change frontend port in `vite.config.js`
3. Update CORS settings and `.env` file accordingly

### Migration Errors

**Problem**: `dotnet ef` command not found

**Solutions**:
1. Install EF Core tools:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
2. Update tools:
   ```bash
   dotnet tool update --global dotnet-ef
   ```

## Production Deployment

### Backend

1. **Build for production**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Configure production settings**
   - Update `appsettings.json` with production connection string
   - Set strong JWT secret key
   - Configure HTTPS

### Frontend

1. **Build for production**
   ```bash
   npm run build
   ```

2. **Deploy `dist` folder** to your web server (IIS, Nginx, etc.)

## New Features in v2.0.0

### User Interface
- **Governmental Design**: Professional institutional interface
- **Sidebar Navigation**: Retractable sidebar with all features
- **Live Currency Rates**: Real-time exchange rates in header
- **Notification System**: Global notifications with unread count
- **Settings Page**: Profile management and card activation

### Security Enhancements
- **Transaction Confirmation Codes**: Secure transaction system
- **Card Activation**: Control card usage with activation toggle
- **Enhanced Validation**: Improved error messages with tips

### Banking Features
- **Dedicated Transactions Page**: Comprehensive transaction management
- **Automatic Balance Updates**: Real-time balance tracking
- **Transaction Blocking**: Inactive cards cannot be used

## Security Notes

⚠️ **Important for Production**:
- Change JWT secret key in `appsettings.json`
- Use proper encryption for credit card data (currently using Base64 for demo)
- Enable HTTPS
- Implement rate limiting
- Add input sanitization
- Use environment variables for sensitive data
- Configure OAuth credentials (Google, Facebook) in `appsettings.json`

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Vue.js 3 Documentation](https://vuejs.org/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Pinia Documentation](https://pinia.vuejs.org/)

