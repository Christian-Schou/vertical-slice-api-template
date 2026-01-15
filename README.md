# Vertical Slice Api Template

This repository provides a template for building applications using the Vertical Slice Architecture with .NET 10. It
includes implementations for Wolverine, Marten (PostgreSQL), Global Exception Handling, Carter, and FluentValidation.

## Table of Contents

- [Getting Started](#getting-started)
- [Architecture Overview](#architecture-overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Folder Structure](#folder-structure)
- [Key Capabilities](#key-capabilities-guide)
- [API Usage Examples](#api-usage-examples)
- [Setup](#setup)
- [Contributing](#contributing)
- [License](#license)
- [Support](#support)

## Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (Required for Marten/PostgreSQL and Integration Tests)

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/Christian-Schou/vertical-slice-api-template.git
   ```
2. Navigate to the project directory
   ```sh
   cd vertical-slice-api-template
   ```
3. Restore dependencies
   ```sh
   dotnet restore
   ```

## Architecture Overview

This template follows the **Vertical Slice Architecture**, which organizes code by features rather than technical concerns.
Each feature is self-contained in the `src/TWC.API/Features` folder, promoting high cohesion and low coupling. You will find requests, validation, handlers, and endpoints for a specific feature all in one place.

## Features

- **Wolverine**: Implements the command bus pattern for handling requests and messages. It integrates deeply with Marten for outbox patterns and durable messaging.
- **Marten**: .NET Transactional Document DB and Event Store on PostgreSQL. Replaces traditional ORMs like EF Core for a more aggregate-centric approach.
- **Identity & Authentication**: Full ASP.NET Core Identity implementation using Marten as the store. Includes JWT Bearer authentication with Refresh Token flow.
- **Global Exception Handling**: Centralized handling of exceptions via `GlobalExceptionHandler`.
- **Carter**: Library for building HTTP APIs with cleaner routing registration.
- **FluentValidation**: Robust validation rules for your commands and requests.
- **FluentResults**: Used to maintain a functional approach to handling success and failure flows.
- **OpenTelemetry**: Integrated observability for distributed tracing and metrics.
- **Structured Logging (Serilog)**: Enhanced logging capabilities.
- **Testcontainers**: Used in Integration Tests to spin up real PostgreSQL instances.

## Technologies Used

- **.NET 10**
- **Wolverine**
- **Marten (PostgreSQL)**
- **Marten.AspNetIdentity**
- **ASP.NET Core Identity**
- **Carter**
- **FluentValidation**
- **FluentResults**
- **OpenTelemetry**
- **Serilog**
- **Microsoft.FeatureManagement**
- **xUnit** (Testing)
- **Shouldly** (Assertions)
- **Testcontainers** (Integration Testing)

## Folder Structure

The solution is structured as follows:

- **src/**
    - **TWC.API**: The main entry point and "host" of the application.
        - **Features**: The heart of the application.
            - **Auth**: Authentication features.
                - **Login**: Endpoint, Request, Response.
                - **Register**: Endpoint, Request.
                - **Refresh**: Endpoint, Request, Response.
            - **Products**: Example feature slice.
                - **CreateProduct**: Command, Validator, Handler, Endpoint.
                - **GetProductById**: Query, Handler, Endpoint.
                - ...
        - **Entities**: Domain entities (Aggregates).
        - **Configurations**: Infrastructure setup (Wolverine, Marten, etc.).
    - **TWC.SharedKernel**: Shared domain primitives (Entity base classes, Result pattern extensions).
    - **TWC.ServiceDefaults**: OpenTelemetry and Service Discovery defaults (Aspire-ready).

- **tests/**
    - **TWC.UnitTests**: Fast, isolated unit tests.
    - **TWC.IntegrationTests**: End-to-end feature tests using Docker containers.
    - **TWC.Architecture.Tests**: Enforces architectural rules (e.g., Domain layer dependencies).

## Key Capabilities Guide

### Domain Events with Wolverine & Marten

We leverage the power of **Marten** and **Wolverine** to handle domain events seamlessly without boilerplate.

Instead of maintaining a complex `IDomainEvent` infrastructure within the entities, we adopt a more functional approach where Handlers allow side-effects (events) to be propagated by Wolverine.

**How it works:**
1. **Define the Event**: Create a simple record (e.g., `ProductCreatedEvent`).
2. **Publish the Event**: The Command Handler returns the event alongside the result. Wolverine automatically detects it and publishes it.
3. **Handle the Event**: Create a handler class with a `Handle` method accepting the event.

**1. Publishing an Event (Create Product):**

```csharp
// Handler returns a Tuple: (Result, Event)
public (Result<CreateProductResult>, ProductCreatedEvent) Handle(CreateProductCommand command)
{
    // 1. Domain Logic
    var product = Product.Create(command.Name, /*...*/);
    
    // 2. Persistence (Marten)
    session.Store(product);
    // Note: No need to call session.SaveChanges(). Wolverine's transactional middleware handles this automatically.

    // 3. Return Result + Event
    // Wolverine will automatically publish 'ProductCreatedEvent' to any subscribers
    return (new CreateProductResult(product.Id), new ProductCreatedEvent(product.Id));
}
```

**2. Handling the Event:**

To react to the event (e.g., send an email, update a read model), simply define a handler class with a `Handle` method. Wolverine automatically discovers and registers it.

```csharp
public sealed class ProductCreatedHandler(ILogger<ProductCreatedHandler> logger)
{
    // Wolverine calls this method when ProductCreatedEvent is published
    public void Handle(ProductCreatedEvent notification)
    {
        // Respond to the event (side-effect)
        logger.LogInformation("Domain Event: Product created with Id {ProductId}", notification.Id);
    }
}
```

This keeps the Domain Entities clean and focuses on business logic.

### Structured Logging with Serilog

The template uses Serilog for structured logging.
Configuration is in `appsettings.json` and code setup in `Program.cs`.

### Feature Flagging

Feature flags are implemented using `Microsoft.FeatureManagement`.

```json
"FeatureManagement": {
  "NewProductFeature": true
}
```

## API Usage Examples

Below are practical examples of how to interact with the API (e.g., using `curl`).

### 1. Create a Product
**POST** `/api/products`

```bash
curl -X POST http://localhost:5000/api/products \
   -H 'Content-Type: application/json' \
   -d '{
         "name": "High-End Laptop",
         "description": "Powerful gaming laptop with 32GB RAM",
         "price": 2500.00,
         "categories": ["Electronics", "Computers"]
       }'
```

### 2. Get Product By Id
**GET** `/api/products/{id}`

```bash
curl -X GET http://localhost:5000/api/products/YOUR_PRODUCT_ID
```

### 3. Update a Product
**PUT** `/api/products`

```bash
curl -X PUT http://localhost:5000/api/products \
   -H 'Content-Type: application/json' \
   -d '{
         "id": "YOUR_PRODUCT_ID",
         "name": "High-End Laptop (Updated)",
         "description": "Now with 64GB RAM",
         "price": 2700.00,
         "categories": ["Electronics", "Computers", "Gaming"]
       }'
```

### 4. Delete a Product
**DELETE** `/api/products/{id}`

```bash
curl -X DELETE http://localhost:5000/api/products/YOUR_PRODUCT_ID
```

## Setup

1. Ensure **Docker** is running (required for Marten database connection).
2. Configure your connection string in `appsettings.json` if using a specific instance, or rely on defaults/Aspire.
3. Run the application:
   ```sh
   dotnet run --project src/TWC.API/TWC.API.csproj
   ```

## Contributing

Contributions are welcome!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

This project is originally based on the Vertical Slice Architecture Template by Poorna Soysa, but has been re-written. [Vertical Slice Architecture Template by Poorna
Soysa](https://github.com/poorna-soysa/vertical-slice-architecture-template)