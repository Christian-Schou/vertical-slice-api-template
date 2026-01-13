# Vertical Slice Architecture Template

This repository provides a template for building applications using the Vertical Slice Architecture with .NET 10. It includes implementations for Wolverine, global exception handling, Carter library, FluentValidation, EF Core, and FluentResults.

## Table of Contents

- [Getting Started](#getting-started)
- [Architecture Overview](#architecture-overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Folder Structure](#folder-structure)
- [Setup](#setup)
- [Contributing](#contributing)
- [License](#license)
- [Support](#support)

## Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/poorna-soysa/vertical-slice-architecture-template.git
   ```
2. Navigate to the project directory
   ```sh
   cd vertical-slice-architecture-template
   ```
3. Restore dependencies
   ```sh
   dotnet restore
   ```

## Architecture Overview

This template follows the Vertical Slice Architecture, which organizes code by features rather than technical concerns. Each feature is self-contained, promoting high cohesion and low coupling.

## Features

- **Wolverine Library**: Implements the command bus pattern for handling requests and messages.
- **Global Exception Handling**: Centralized handling of exceptions.
- **Carter Library**: Lightweight library for building HTTP APIs.
- **FluentValidation Library**: Validation library for .NET.
- **FluentResults Library**: Implements the Result pattern.
- **Health Checks**: Standardized approach for monitoring and assessing the operational status of systems.
- **OpenTelemetry**: Integrated observability for distributed tracing and metrics.
- **Structured Logging (Serilog)**: Enhanced logging capabilities with Serilog.
- **Feature Management**: Feature flagging support using Microsoft.FeatureManagement.

## Technologies Used

- **.NET 10**
- **Wolverine**
- **Carter Library**
- **FluentValidation**
- **FluentResults**
- **EF Core**
- **HealthChecks Library**
- **OpenTelemetry**
- **Serilog**
- **Microsoft.FeatureManagement**
- **xUnit**
- **NSubstitute**
- **Shouldly**

## Key Capabilities Guide

### Structured Logging with Serilog

The application is configured to use Serilog for structured logging, replacing the default ASP.NET Core logger.

**Configuration:**
- Code configuration is located in `VSATemplate/Configurations/SerilogConfiguration.cs`.
- Request logging is enabled in `Program.cs`.

**Customization:**
Adjust log levels in `appsettings.json`:
```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    }
  }
}
```

### Feature Flagging

Feature flags are implemented using `Microsoft.FeatureManagement`, allowing you to enable/disable features without code changes.

**1. Define Flags**
Add flags to the `FeatureManagement` section in `appsettings.json` or `appsettings.Development.json`:

```json
"FeatureManagement": {
  "NewProductFeature": true,
  "BetaFeature": false
}
```

**2. Usage in Code**
Inject `IFeatureManager` into your Classes, Handlers, or Services:

```csharp
public class GetProductHandler(IFeatureManager featureManager)
{
    public async Task<Result<ProductResponse>> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        if (await featureManager.IsEnabledAsync("NewProductFeature"))
        {
            // Execute new logic
        }

        // Execute default logic
    }
}
```

### OpenTelemetry

OpenTelemetry is pre-configured for Tracing and Metrics.

- **Tracing**: Includes ASP.NET Core, HTTP Client, Entity Framework Core, and Wolverine.
- **Metrics**: Includes ASP.NET Core, HTTP Client, and Wolverine.

Configuration is located in `VSATemplate/Extensions/OpenTelemetryExtensions.cs`.

## Folder Structure

- **/VSATemplate**: Contains the main application code.
  - **/Features**: Each feature is organized into its own folder, promoting encapsulation.
    - **/Products**: Contains all product related files for the feature.
       - **/CreateProduct**:  Logic for creating a product.
       - **/DeleteProduct**: Logic for deleting a product.
       - **/GetProductById**: Logic for retrieving a product by its Id.
       - **/GetProducts**: Logic for retrieving a list of products.
       - **/UpdateProduct**: Logic for updating product details.
       - **ProductErrors**: Contains all product-related error handling.
  - **/Abstractions**: Contains shared interfaces and contracts.
  - **/Database**: Contains database-related code, including DB Context.
  - **/Entities**: Defines the core data models used throughout the application.
  - **/Exceptions**: Contains the global exception handler for the application.
  - **/Extensions**: Contains extension methods for various classes and services.
  - **/Migrations**: Database migration files for schema updates.
  - **Program.cs**: Application entry point.

- **/VSATemplate.UnitTests**: Contains unit tests for the application logic.
- **/VSATemplate.IntegrationTests**: Contains integration tests for the features.
- **/VSATemplate.Architecture.Tests**: Contains architecture tests to enforce design rules.


## Setup

1. Configure your database connection string in `appsettings.json`.
2. Run the application
   ```sh
   dotnet run
   ```

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE` for more information.

This project is originally based on the Vertical Slice Architecture Template by Poorna Soysa. https://github.com/poorna-soysa/vertical-slice-architecture-template