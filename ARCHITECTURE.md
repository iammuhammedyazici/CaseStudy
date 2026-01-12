# E-Commerce Microservices Architecture

This document provides a comprehensive overview of the architecture, design patterns, and technologies used in the E-Commerce Case Study project.

## 1. High-Level Overview

The solution is a **Microservices-based**, **Event-Driven** architecture built with **.NET 8**. It leverages modern practices such as **Clean Architecture**, **CQRS**, **Saga Pattern**, and **Result Pattern** to ensure scalability, maintainability, and reliability.

### Core Technologies
- **Framework**: .NET 8
- **Database**: PostgreSQL (Database-per-service)
- **Messaging**: RabbitMQ (via MassTransit)
- **Caching**: Redis (Distributed Cache)
- **ORM**: Entity Framework Core 9
- **Search Engine**: Elasticsearch
- **Observability**: OpenTelemetry, Serilog, Aspire Dashboard
- **Containerization**: Docker & Docker Compose
- **API Gateway**: YARP (Yet Another Reverse Proxy)

---

## 2. Solution Structure

The repository is organized into the following key areas:

- **`src/ApiGateway`**: The entry point for all external client requests.
- **`src/Services`**: Independent microservices (Order, Stock, Product, Notification).
- **`src/BuildingBlocks`**: Shared libraries for cross-cutting concerns.
- **`deploy`**: Infrastructure configuration (Docker Compose).

### Microservices
| Service | Type | Responsibility | Database |
|---------|------|----------------|----------|
| **Order Service** | API | Order lifecycle management, Saga State tracking. | `OrderDb` |
| **Stock Service** | API + Worker | Inventory management, Stock reservation. | `StockDb` |
| **Product Service** | API | Product catalog management, Search (Elasticsearch). | `ProductDb` |
| **Notification Service** | Worker | Multi-channel notifications (Email/SMS) with SOLID architecture. | `NotificationDb` |

---

## 3. Architectural Patterns

### 3.1. Clean Architecture
Each service follows the Clean Architecture principles to separate concerns:
- **Domain**: Core entities, value objects, and business rules. (No dependencies)
- **Application**: Use cases (CQRS Handlers), DTOs, interfaces. (Depends on Domain)
- **Infrastructure**: External concerns (Db, Message Bus, Consumers). (Depends on Application)
- **Api / Worker**: Entry point, Configuration, Controllers. (Depends on Application & Infrastructure)

**Key Principle: Thin Controllers**
- Controllers have **no business logic**
- They only:
  1. Accept requests (DTOs or Commands/Queries directly)
  2. Dispatch to MediatR
  3. Return standardized responses via `Result<T>.ToActionResult()`

### 3.2. CQRS (Command Query Responsibility Segregation)
The **MediatR** library is used to decouple command/query execution.
- **Commands**: Modify state (e.g., `CreateOrderCommand`)
- **Queries**: Read state (e.g., `GetOrderQuery`)
- **Handlers**: Contain all business logic, validation, and authorization
- **Behaviors**: Cross-cutting concerns like Validation (`FluentValidation`) and Logging are implemented as MediatR Pipeline Behaviors

### 3.3. Result Pattern
All handlers return `Result<T>` or `Result` (from `ECommerce.Contracts.Common`) instead of throwing exceptions for business failures.

**Benefits:**
- Explicit error handling
- No try-catch in controllers
- Standardized API responses (200, 400, 404, etc.)

**Usage:**
```csharp
// Handler
public async Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken ct)
{
    var order = await _repository.GetByIdAsync(request.OrderId);
    if (order == null)
        return Result<OrderDto>.NotFound("Order not found");
    
    return Result<OrderDto>.Success(orderDto);
}

// Controller
public async Task<IActionResult> GetOrder(Guid id)
{
    var result = await _mediator.Send(new GetOrderQuery(id));
    return this.ToActionResult(result); // Extension method
}
```

### 3.4. Saga Pattern (Hybrid Approach)
Complex business transactions that span multiple services are managed using a **Hybrid Saga** approach:

**State Machine (Orchestration Element):**
- `OrderStateMachine` (MassTransit State Machine) tracks the order lifecycle
- State is persisted in `OrderState` table (PostgreSQL)
- Provides centralized visibility and observability

**Event-Driven Communication (Choreography Element):**
- Services communicate via **Events**, not Commands
- No direct coupling between services
- Each service reacts to events autonomously

**Flow:**
1. `OrderCreatedEvent` → Saga transitions to `Submitted` state
2. Stock Worker consumes event → Reserves stock
3. `StockReservedEvent` → Saga transitions to `Confirmed` state
4. `StockReservationFailedEvent` → Saga transitions to `Rejected` state

**Why Hybrid?**
- **Observability**: Centralized state tracking (easier debugging)
- **Loose Coupling**: Event-based communication (no service-to-service HTTP calls)
- **Flexibility**: Services can be added/removed without changing the Saga

### 3.5. Outbox & Inbox Patterns
To ensure **Consistency** and **Idempotency** in a distributed system:

**Transactional Outbox:**
- Domain events are saved to the database in the **same transaction** as business data
- Background publisher reads from Outbox table and publishes to RabbitMQ
- Guarantees **at-least-once delivery**
- Prevents dual-write problem (DB write + Event publish atomicity)

**Inbox:**
- Consumers track processed `MessageId` in Inbox table
- Duplicate messages are automatically ignored
- Ensures **exactly-once processing** (idempotency)

### 3.6. Authentication & Authorization
**JWT Bearer Authentication:**
- Validated at API Gateway
- Claims extracted and propagated to downstream services via headers

**Authorization in Handlers:**
- `ICurrentUserService` provides `UserId` and `IsAdmin` from HTTP context
- Handlers perform authorization checks (e.g., "Can this user access this order?")
- Returns `Result.Unauthorized()` or `Result.Forbidden()` for access violations

**Example:**
```csharp
public class GetOrderHandler : IRequestHandler<GetOrderQuery, Result<OrderDto>>
{
    private readonly ICurrentUserService _currentUser;
    
    public async Task<Result<OrderDto>> Handle(...)
    {
        var order = await _repository.GetByIdAsync(request.OrderId);
        
        // Authorization check
        if (!_currentUser.IsAdmin && order.UserId != _currentUser.UserId)
            return Result<OrderDto>.Forbidden("Access denied");
        
        return Result<OrderDto>.Success(orderDto);
    }
}
```

### 3.7. Centralized Error Handling
- Implemented using .NET 8 `IExceptionHandler`
- **`GlobalExceptionHandler`**: Catches unhandled exceptions, logs them securely, and returns a standardized **RFC 7807 ProblemDetails** response
- Configured globally in `ECommerce.Observability`

---

## 4. Communication & Messaging

### MassTransit
MassTransit is used as the abstraction layer over RabbitMQ.
- **Configuration**: Centralized in `ECommerce.Messaging`
- **Transports**: RabbitMQ
- **Features**: Automatic retries, Dead Letter Queues (DLQ), Serialization, Topology management, State Machine support

### Event Flow (Order Creation)

```
┌─────────┐      ┌──────────────┐      ┌──────────┐
│ Client  │─────▶│ API Gateway  │─────▶│ Order API│
└─────────┘      └──────────────┘      └─────┬────┘
                                              │
                                              ▼
                                      ┌───────────────┐
                                      │ Order DB      │
                                      │ - Order       │
                                      │ - Outbox      │
                                      │ - OrderState  │
                                      └───────┬───────┘
                                              │
                                              ▼
                                      ┌───────────────┐
                                      │ RabbitMQ      │
                                      │ OrderCreated  │
                                      └───┬───────┬───┘
                                          │       │
                        ┌─────────────────┘       └─────────────────┐
                        ▼                                           ▼
                ┌───────────────┐                         ┌─────────────────┐
                │ Stock Worker  │                         │ OrderStateMachine│
                └───────┬───────┘                         │ (Saga)          │
                        │                                 └─────────────────┘
                        ▼
                ┌───────────────┐
                │ Stock DB      │
                │ - Stock       │
                │ - Inbox       │
                └───────┬───────┘
                        │
                        ▼
                ┌───────────────┐
                │ RabbitMQ      │
                │ StockReserved │
                └───┬───────┬───┘
                    │       │
    ┌───────────────┘       └─────────────────┐
    ▼                                         ▼
┌─────────────────┐                  ┌──────────────────┐
│ Order API       │                  │ Notification     │
│ (Saga Consumer) │                  │ Worker           │
└─────────────────┘                  └──────────────────┘
```

**Detailed Steps:**
1. **Client** POSTs to `/api/orders` (via Gateway)
2. **Order API** creates `Order` entity (Pending status) and saves to DB with Outbox entry (same transaction)
3. **Outbox Publisher** reads Outbox and publishes `OrderCreatedEvent` to RabbitMQ
4. **OrderStateMachine** consumes `OrderCreatedEvent` and transitions to `Submitted` state
5. **Stock Worker** consumes `OrderCreatedEvent`, checks inventory:
   - Saves to Inbox (idempotency check)
   - *Success*: Reserves stock, publishes `StockReservedEvent`
   - *Failure*: Publishes `StockReservationFailedEvent`
6. **OrderStateMachine** consumes the result:
   - *Success*: Transitions to `Confirmed` state
   - *Failure*: Transitions to `Rejected` state
7. **Notification Worker** consumes stock events and logs notifications

---

## 5. Observability

The system is designed to be fully observable using **OpenTelemetry**.

- **Tracing**: Distributed tracing across all services (ASP.NET Core, HttpClient, MassTransit, EF Core)
- **Metrics**: Runtime metrics, custom business metrics
- **Logging**: Structured logging with **Serilog**, enriched with `TraceId`, `SpanId`, and `CorrelationId` for correlation
- **Visualization**: All telemetry data is exported to the **Aspire Dashboard** (OTLP endpoint)

**Key Features:**
- `CorrelationId` propagation across services
- `UserContextMiddleware` enriches logs with `UserId`
- Centralized log aggregation (compatible with Seq, ELK, etc.)

---

## 6. Security

- **Authentication**: JWT Bearer Authentication (validated at Gateway)
- **Authorization**: 
  - Role-based access control (RBAC) with policies (`AdminOnly`, `CustomerOnly`)
  - Fine-grained authorization in handlers via `ICurrentUserService`
- **Identity**: Token generation endpoint (Dev) and validation logic
- **Secrets Management**: Configuration via `appsettings.json` (production: use Vault/K8s Secrets)

---

## 7. Infrastructure (Docker Compose)

The `deploy/docker-compose.yml` file orchestrates the entire environment:
- **Services**: Order API, Stock API, Stock Worker, Product API, Notification Worker, API Gateway
- **Databases**: PostgreSQL (4 separate databases)
- **Broker**: RabbitMQ Management
- **Search**: Elasticsearch
- **Cache**: Redis
- **Observability**: Aspire Dashboard

**Health Checks:**
- All services expose `/health` endpoints
- Docker Compose uses health checks for dependency management

---

## 8. Development Standards

- **Code Style**: Follows standard C# conventions
- **Testing**: xUnit, Moq, FluentAssertions
- **Validation**: FluentValidation (via MediatR Pipeline Behavior)
- **Mapping**: Manual mapping via extension methods (no AutoMapper for clarity)
- **Error Handling**: Result Pattern (no exceptions for business logic failures)
- **Configuration**: Externalized to `appsettings.json`, fail-fast on missing values

---

## 9. Notification Service (SOLID Architecture)

The Notification Service demonstrates **SOLID principles** in practice, with a clean separation of concerns and extensibility for multiple notification channels.

### Architecture Layers

**Domain Layer:**
- `NotificationChannel` enum (Email, SMS, Push)
- `NotificationRequest` value object (immutable)

**Application Layer:**
- `INotificationService`: Main orchestrator interface
- `IEmailService`: Email-specific abstraction
- `ISmsService`: SMS-specific abstraction
- `SendNotificationHandler`: MediatR handler (delegates to `INotificationService`)

**Infrastructure Layer:**
- `NotificationService`: Orchestrator implementation (routes to correct channel)
- `SendGridEmailService`: Real email provider (SendGrid SDK)
- `TwilioSmsService`: Real SMS provider (Twilio SDK)
- `MockEmailService`: Test implementation (logs to console)
- `MockSmsService`: Test implementation (logs to console)

### SOLID Principles Applied

| Principle | Implementation |
|-----------|----------------|
| **SRP** | Each service has one responsibility (e.g., `SendGridEmailService` only sends emails) |
| **OCP** | New channels (Push, WhatsApp) can be added without modifying existing code |
| **LSP** | `MockEmailService` and `SendGridEmailService` are interchangeable |
| **ISP** | Separate interfaces for Email, SMS, and orchestration |
| **DIP** | All dependencies are on abstractions (`IEmailService`, `ISmsService`) |

### Configuration (Feature Flag)

```json
{
  "NotificationSettings": {
    "UseMockProviders": true  // true = Mock, false = Real providers
  },
  "SendGrid": {
    "ApiKey": "",
    "FromEmail": "noreply@ecommerce.com"
  },
  "Twilio": {
    "AccountSid": "",
    "AuthToken": "",
    "FromPhoneNumber": "+15551234567"
  }
}
```

**Default Behavior:**
- Mock providers are enabled by default (no API keys required)
- Logs to console: `[MOCK EMAIL] To: user@example.com, Subject: ...`
- Notifications are still logged to `NotificationLogs` table

**Production Setup:**
1. Add SendGrid/Twilio API keys to `appsettings.json`
2. Set `UseMockProviders: false`
3. Restart `notification-worker`

---

## 10. Key Design Decisions

### Why Event-Driven over Synchronous HTTP?
- **Resilience**: Services don't fail if dependencies are down
- **Scalability**: Message queues buffer load spikes
- **Decoupling**: Services evolve independently

### Why Hybrid Saga (not pure Choreography)?
- **Observability**: Centralized state makes debugging easier
- **Complexity Management**: Easier to understand flow than pure event chains

### Why Result Pattern over Exceptions?
- **Performance**: No exception overhead for expected failures
- **Clarity**: Explicit error handling in code
- **API Design**: Clean HTTP status code mapping

### Why Thin Controllers?
- **Testability**: Business logic in handlers (easy to unit test)
- **Reusability**: Same handler can be called from API, Worker, or CLI
- **Maintainability**: Controllers become pure routing/dispatching

---

## 11. Future Enhancements

- **Circuit Breaker**: Polly for resilience
- **Rate Limiting**: API Gateway rate limiting
- **API Versioning**: URL or header-based versioning
- **Distributed Caching**: Redis for read-heavy queries
- **Event Sourcing**: For audit trail and temporal queries
- **gRPC**: For high-performance internal service communication
