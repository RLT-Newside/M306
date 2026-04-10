# Fitness Check API

A comprehensive fitness assessment and tracking system for the GIBZ (Gewerblich-industrielle Berufsschule Zug) designed to manage and evaluate student fitness performance across six different sports disciplines.

## 🏃‍♂️ Overview

The Fitness Check API is a RESTful web service that enables students and instructors to track, record, and analyze fitness test results across multiple disciplines. The system provides detailed performance analytics, annual progress tracking, and comparative class statistics to support comprehensive fitness assessment in educational environments.

## 🏆 Supported Disciplines

The system tracks performance across six standardized fitness disciplines:

- **Core Strength**
- **Medicine Ball Push**
- **Standing Long Jump**
- **One Leg Stand**
- **Shuttle Run**
- **Twelve Minutes Run**

Each discipline is scored using standardized point tables (0-25 points) with gender-specific evaluation criteria.

## 🛠️ Technical Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: MySQL/MariaDB with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Documentation**: OpenAPI/Swagger with Scalar UI
- **Mapping**: AutoMapper for entity-DTO transformations
- **Containerization**: Docker with Docker Compose

## 📋 Key Features

### Performance Tracking

- Record multiple attempts per discipline with configurable limits
- Automatic best attempt identification and point calculation
- School year-based performance grouping and analysis
- Annual performance progression tracking

### Analytics & Reporting

- Individual student performance overviews
- Cohort average comparisons
- Year-over-year performance analysis
- Detailed attempt history with timestamps

### Data Management

- Automated cohort and gender service integration
- Configurable maximum attempts per discipline
- Comprehensive audit trails for all fitness activities
- Support for both current and historical data analysis

## 🚀 Quick Start

### Prerequisites

- Docker & Docker Compose
- .NET 9.0 SDK (for development)
- Google Cloud credentials (for authentication)

### Running with Docker

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd fitness-check
   ```

2. **Set up Google Cloud credentials**

   ```bash
   # Place your Google Cloud credentials file
   cp path/to/your/credentials.json google_application_default_credentials.json
   ```

3. **Start the services**

   ```bash
   docker compose up --build
   ```

4. **Access the application**
   - API: http://localhost:8008
   - API Documentation: http://localhost:8008/scalar/v1
   - Database: localhost:13308

### Development Setup

1. **Install dependencies**

   ```bash
   cd src
   dotnet restore
   ```

2. **Configure user secrets**

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:MariaDb" "your-connection-string"
   ```

3. **Run migrations**

   ```bash
   dotnet ef database update
   ```

4. **Start development server**
   ```bash
   dotnet watch run
   ```

### Generating JWT Tokens for Local Development

For local development and testing, you'll need to generate JWT tokens to authenticate API requests. The application expects specific claims in the JWT payload.

#### Required Claims

- **`sub` (Subject)**: Any valid UUID - represents the user ID
- **`upn` (User Principal Name)**: A valid GIBZ email address (e.g., `hmueller3@online.gibz.ch`)

#### Using dotnet user-jwts Tool

1. **Install the user-jwts tool** (if not already available):
   ```bash
   dotnet tool install --global Microsoft.AspNetCore.Authentication.JwtBearer.Tools
   ```

2. **Generate a token**:
   ```bash
   dotnet user-jwts create \
     --audience "fitness-check-api" \
     --issuer "dotnet-user-jwts" \
     --claim "sub=550e8400-e29b-41d4-a716-446655440000" \
     --claim "upn=hmueller3@online.gibz.ch" \
     --project src/FitnessCheck.csproj
   ```

3. **Use the token**: Copy the generated token and include it in your API requests:
   ```bash
   curl -H "Authorization: Bearer YOUR_JWT_TOKEN" \
        http://localhost:8008/api/v2/core-strength
   ```

#### Manual Token Generation

Alternatively, you can use online JWT generators like [jwt.io](https://jwt.io) with the following payload:
```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "upn": "hmueller3@online.gibz.ch",
  "iss": "dotnet-user-jwts",
  "aud": "fitness-check-api",
  "exp": 1735689600
}
```

> **Note**: Make sure the signing key matches the one configured in your application settings (`Authentication__Schemes__Bearer__SigningKeys__0__Value`).

## 📡 API Endpoints

### Core Endpoints

#### Attempt Management (v2)

```
GET    /api/v2/{discipline}           # Get attempts for a discipline
POST   /api/v2/{discipline}           # Create new attempt
GET    /api/v2/{discipline}/{year}    # Get attempts for specific year
```

#### Overview & Analytics

```
GET    /api/v2/overview              # Get complete yearly overview
GET    /api/v2/overview/{year}       # Get specific year overview
```

#### Health & Monitoring

```
GET    /health/readiness             # Application readiness check
```

### Supported Disciplines

- `core-strength`
- `medicine-ball-push`
- `standing-long-jump`
- `one-leg-stand`
- `shuttle-run`
- `twelve-minutes-run`

## 🗃️ Database Schema

### Core Entities

- **DisciplineAttempt** - Base class for all fitness attempts
- **Cohort** - Student groupings and class information
- **BestAttempt** - Optimized best performance tracking
- **ResultsCalculation** - Scoring and grading matrices

### Attempt Types

Each discipline has a specific attempt entity inheriting from `DisciplineAttempt<TResultValue>`:

- `CoreStrengthAttempt` (uint)
- `MedicineBallPushAttempt` (uint)
- `StandingLongJumpAttempt` (uint)
- `OneLegStandAttempt` (uint)
- `ShuttleRunAttempt` (uint)
- `TwelveMinutesRunAttempt` (float)

## ⚙️ Configuration

### Application Settings

```json
{
  "FitnessCheck": {
    "MaxAllowedAttempts": {
      "CoreStrength": 3,
      "MedicineBallPush": 3,
      "OneLegStand": 3,
      "ShuttleRun": 2,
      "StandingLongJump": 3,
      "TwelveMinutesRun": 1
    }
  }
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `ConnectionStrings__MariaDb` - Database connection string
- `GOOGLE_APPLICATION_CREDENTIALS` - Path to Google Cloud credentials
- `Authentication__Schemes__Bearer__SigningKeys__0__Value` - JWT signing key

## 🙏 Acknowledgments

This project was initially developed in 2024 by Pascal Bitterli, Alaxsan Jeyakumar and Timo Oswald. Futher development is based on the work of Adrian Alves, Lukas Wyss and Tobias Gretler. Thank you for your contribution!

## 📞 Support

For questions, issues, or feature requests, please contact the development team or create an issue in the project repository.
