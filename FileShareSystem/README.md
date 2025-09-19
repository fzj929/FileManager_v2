# FileShare System

A secure file sharing system built with C# .NET 8 Web API and Vue.js 3 frontend.

## Features

- **User Authentication**: Registration and login with JWT tokens
- **File Management**: Upload, download, and delete personal files
- **File Sharing**: Create secure share links with optional expiration dates
- **Security**: JWT authentication, CORS configured, file access control
- **Modern UI**: Clean and responsive interface using Element Plus

## Tech Stack

### Backend
- **.NET 8** Web API
- **SQLite** Database
- **Entity Framework Core** ORM
- **ASP.NET Core Identity** User management
- **JWT** Authentication
- **Swagger** API documentation

### Frontend
- **Vue.js 3** with Composition API
- **TypeScript**
- **Element Plus** UI components
- **Pinia** State management
- **Vue Router** Navigation
- **Axios** HTTP client

## Project Structure

```
FileShareSystem/
├── FileShareAPI/              # .NET Web API
│   ├── Controllers/           # API controllers
│   ├── Data/                  # Database context
│   ├── Models/                # Data models
│   ├── Services/              # Business logic
│   └── Uploads/               # File storage
├── vue-frontend/              # Vue.js frontend
│   ├── src/
│   │   ├── components/        # Reusable components
│   │   ├── views/             # Page components
│   │   ├── stores/            # Pinia stores
│   │   ├── services/          # API services
│   │   ├── router/            # Vue Router config
│   │   └── types/             # TypeScript types
│   └── public/                # Static assets
└── README.md
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- SQLite (included via Entity Framework)

### Backend Setup

1. Navigate to the API directory:
```bash
cd FileShareAPI
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the API:
```bash
dotnet run
```

The API will start at `https://localhost:5000` (HTTP on `http://localhost:5001`)

API documentation available at: `https://localhost:5000/swagger`

### Frontend Setup

1. Navigate to the frontend directory:
```bash
cd vue-frontend
```

2. Install dependencies:
```bash
npm install
```

3. Run the development server:
```bash
npm run dev
```

The frontend will start at `http://localhost:5173`

## API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/logout` - User logout

### Files
- `GET /api/files` - Get user's files
- `POST /api/files/upload` - Upload file
- `GET /api/files/download/{id}` - Download file
- `DELETE /api/files/{id}` - Delete file
- `POST /api/files/{id}/share` - Share file
- `GET /api/files/shared/{token}` - Access shared file

## Configuration

### Backend Configuration (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=fileshare.db"
  },
  "Jwt": {
    "Key": "your-super-secret-jwt-key",
    "Issuer": "FileShareAPI",
    "Audience": "FileShareUsers",
    "ExpireMinutes": 1440
  },
  "FileStorage": {
    "UploadPath": "Uploads",
    "MaxFileSize": 104857600
  }
}
```

### Frontend Configuration (`vite.config.ts`)
- Proxy configured for API calls to `http://localhost:5000`
- CORS enabled for development

## Security Features

- JWT-based authentication
- Password hashing with ASP.NET Core Identity
- File access control (users can only access their own files)
- Secure file sharing with optional expiration
- CORS configured for specific origins
- Input validation and sanitization

## File Storage

- Files stored in `FileShareAPI/Uploads/` directory
- Each file renamed with GUID to prevent conflicts
- Original filename preserved for user display
- File metadata stored in SQLite database

## Development

### Running Tests
Backend tests:
```bash
cd FileShareAPI
dotnet test
```

### Building for Production
Backend:
```bash
cd FileShareAPI
dotnet publish -c Release
```

Frontend:
```bash
cd vue-frontend
npm run build
```

## Deployment

1. Configure production database connection string
2. Update JWT key and other secrets
3. Set up file storage directory with appropriate permissions
4. Configure CORS for production frontend URL
5. Use reverse proxy (nginx/Apache) for production deployment

## License

This project is open source and available under the MIT License.