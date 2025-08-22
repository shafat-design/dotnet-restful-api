"# .NET RESTful API with Authentication and Authorization

A comprehensive .NET 8 Web API project featuring JWT-based authentication, role-based authorization, and complete user management functionality.

## Features

### 🔐 Authentication & Authorization
- **JWT-based authentication** with secure token generation
- **Role-based authorization** (Admin, Manager, User)
- **Token blacklisting** for secure logout functionality
- **Password hashing** using BCrypt for security
- **Middleware protection** for all routes except registration/login

### 👥 User Management
- Complete CRUD operations for user management
- User registration with role assignment
- Profile management and updates
- Role-based access control with granular permissions

### 📝 Audit Logging
- Automatic **CreatedBy/UpdatedBy** field tracking
- **CreatedAt/UpdatedAt** timestamps
- Full audit trail for all operations

### 🛡️ Security Features
- All routes protected except registration/login
- JWT token validation middleware
- Role-based endpoint protection
- Token expiration and refresh mechanism
- Secure password hashing with BCrypt

### 📚 API Documentation
- **Swagger/OpenAPI** documentation with JWT authentication support
- Interactive API testing interface
- Comprehensive schema documentation

## Technology Stack

- **.NET 8 Web API**
- **Entity Framework Core** with In-Memory database
- **JWT Authentication** (System.IdentityModel.Tokens.Jwt)
- **BCrypt** for password hashing
- **Swagger/OpenAPI** for documentation
- **Role-based Authorization** policies

## Project Structure

```
RestfulApiProject/
├── Controllers/
│   ├── AuthController.cs      # Authentication endpoints
│   └── UsersController.cs     # User management endpoints
├── Services/
│   ├── IAuthService.cs & AuthService.cs           # Authentication logic
│   ├── IUserService.cs & UserService.cs           # User management
│   ├── ITokenService.cs & TokenService.cs         # JWT token handling
│   └── ITokenBlacklistService.cs & TokenBlacklistService.cs # Token blacklist
├── Models/
│   ├── User.cs               # User entity with audit fields
│   └── JwtSettings.cs        # JWT configuration model
├── DTOs/
│   └── UserDto.cs           # Data transfer objects
├── Data/
│   ├── ApplicationDbContext.cs  # Entity Framework context
│   └── SeedData.cs             # Database seeding
├── Program.cs               # Application configuration
├── appsettings.json         # Configuration settings
└── RestfulApiProject.csproj # Project dependencies
```

## API Endpoints

### Authentication Endpoints
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/logout` - User logout (requires authentication)
- `GET /api/auth/profile` - Get current user profile (requires authentication)

### User Management Endpoints
- `GET /api/users` - Get all users (Admin only)
- `GET /api/users/{id}` - Get user by ID (Users can view own profile, Admin/Manager can view any)
- `PUT /api/users/{id}` - Update user (Admin/Manager only, with role restrictions)
- `DELETE /api/users/{id}` - Delete user (Admin only)

## Role-Based Access Control

### User Roles
1. **User (0)** - Basic access to own profile
2. **Manager (1)** - Can update regular users and own profile, cannot assign Admin/Manager roles
3. **Admin (2)** - Full access to all operations

### Permission Matrix
| Endpoint | User | Manager | Admin |
|----------|------|---------|-------|
| Register/Login | ✅ | ✅ | ✅ |
| Own Profile | ✅ | ✅ | ✅ |
| View Other Users | ❌ | ✅ | ✅ |
| Update Users | ❌ | ✅* | ✅ |
| Delete Users | ❌ | ❌ | ✅ |
| View All Users | ❌ | ❌ | ✅ |

*Managers can only update regular users and cannot assign Admin/Manager roles

## Default Test Users

The application seeds three default users for testing:

| Username | Password | Role | Email |
|----------|----------|------|-------|
| admin | Admin123! | Admin | admin@example.com |
| manager | Manager123! | Manager | manager@example.com |
| user | User123! | User | user@example.com |

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code (optional)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd dotnet-restful-api
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the API**
   - API Base URL: `http://localhost:5000`
   - Swagger Documentation: `http://localhost:5000` (root)

## Usage Examples

### 1. User Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "Admin123!"}'
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@example.com",
    "role": 2,
    "createdAt": "2025-08-22T17:03:49.9449751Z",
    "updatedAt": "2025-08-22T17:03:49.9449756Z",
    "createdBy": null,
    "updatedBy": null
  },
  "expiresAt": "2025-08-23T17:04:04.6190528Z"
}
```

### 2. Access Protected Endpoint
```bash
curl -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  http://localhost:5000/api/auth/profile
```

### 3. Register New User
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "newuser",
    "email": "newuser@example.com",
    "password": "NewUser123!",
    "role": 0
  }'
```

### 4. Update User (Manager/Admin only)
```bash
curl -X PUT http://localhost:5000/api/users/1 \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"email": "newemail@example.com"}'
```

## Configuration

### JWT Settings (appsettings.json)
```json
{
  "JwtSettings": {
    "Secret": "YourSecretKeyHere",
    "Issuer": "RestfulApiProject",
    "Audience": "RestfulApiProject",
    "ExpirationHours": 24
  }
}
```

### Security Considerations
- JWT secret keys should be at least 32 characters long
- In production, use environment variables for sensitive configuration
- The in-memory database is for development only - use a persistent database in production

## Swagger Integration

The API includes comprehensive Swagger documentation with:
- Interactive endpoint testing
- JWT authentication support
- Schema documentation for all DTOs
- Example requests and responses

To use JWT authentication in Swagger:
1. Navigate to the Swagger UI at `http://localhost:5000`
2. Click the "Authorize" button
3. Enter `Bearer YOUR_TOKEN_HERE` in the value field
4. Click "Authorize" and then test protected endpoints

## Audit Trail

All user operations automatically track:
- **CreatedAt/UpdatedAt**: Timestamps for creation and modification
- **CreatedBy/UpdatedBy**: User ID of the person performing the action
- Audit fields are automatically populated by the Entity Framework context

## Testing

The project includes comprehensive testing capabilities:
- Unit tests can be added for services and controllers
- Integration tests can test the full API pipeline
- Swagger UI provides manual testing capabilities
- Default test users enable immediate API testing

## Development

### Adding New Endpoints
1. Create DTOs in the `DTOs` folder
2. Add business logic to existing services or create new services
3. Create or extend controllers with new endpoints
4. Apply appropriate authorization attributes
5. Update Swagger documentation if needed

### Database Changes
The project uses Entity Framework Code First with an in-memory database. For production:
1. Replace `UseInMemoryDatabase` with your preferred database provider
2. Add migrations: `dotnet ef migrations add InitialCreate`
3. Update database: `dotnet ef database update`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Support

For questions or issues, please open an issue in the repository or contact the development team." 
