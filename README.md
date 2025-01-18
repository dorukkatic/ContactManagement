
# ContactManagement

## Overview
This repository contains a microservices-based contact management system designed as part of an interview assessment. It emphasizes clean architecture, efficient design choices, and scalable technology while balancing complexity with time constraints. Below are the design decisions, technologies, and features implemented in the project.

---

## Design Choices

### Database
- A single PostgreSQL database is used for the entire project, with separate schemas for each microservice.
- While using separate databases for each microservice or splitting read/write databases is common, this approach was avoided to reduce complexity given the project scope.
- A NoSQL database could be a better fit for the reporting service due to its flexibility for analytics, but it was not implemented to avoid adding unnecessary complexity.

### Architecture
Each microservice follows a layered architecture to ensure a clean separation of concerns and ease future modifications. Layers include:
- **WebApi**: HTTP interface.
- **Application**: Business logic.
- **Domain**: Core entities.
- **DataAccess**: Database operations.
- **Contracts**: DTOs and interfaces.
- **DependencyInjection**: Centralized dependency injection configuration.
- **Tests**: Unit tests.

Key architectural considerations:
- Microservices are implemented without hard dependencies on each other.
- Common patterns like Unit of Work, Repository, CQRS, and MediatR were not used to avoid excessive boilerplate for this project’s scope.
- Advanced microservice patterns like retries, circuit breakers, etc. were omitted due to time constraints.
- Integration and acceptance tests were not implemented because of time limitations.
- Logging and telemetry configurations were skipped for simplicity.

---

## Technologies Used
- .NET 8
- PostgreSQL
- RabbitMQ
- Docker
- YARP Reverse Proxy
- MassTransit
- Entity Framework Core
- FluentValidation
- FluentResults
- Swagger
- xUnit
- Bogus
- NSubstitute

---

## Project Highlights

### Contacts Service
- **Pagination**: Implemented for listing endpoints to support an infinite number of contacts and contact information records.
- **Contact Info Retrieval**: The `GET /api/people/{id}` endpoint returns limited contact info. For full details, the `GET /api/people/{id}/contact-info` endpoint supports pagination.
- **Location Management**: Adding location information is handled separately from other contact details. This design choice avoids redundancy, as multiple locations per contact are unlikely in this domain.
- **Statistics**: Location-based statistics are implemented within the Contacts service to encapsulate domain logic and avoid exposing the Contacts database to the Reports microservice. While a queue-triggered approach could improve performance, REST communication was chosen to align with requirements.

#### Configuration
The only required configuration is the database connection string, which can be set in the `appsettings.json` file:
```json
"ConnectionStrings": {
  "ContactsDb": "Host=localhost;Port=5432;Database=ContactsDb;Username=postgres;Password=setur"
}
```

---

### Reports Service
- **Database Design**: Report data is stored in a `jsonb` column in PostgreSQL to allow flexibility for handling different report types. Parsing is managed in the application layer.
- **Messaging**: MassTransit is used to publish and consume events in RabbitMQ for background report generation.
- **Pagination**: Implemented for report listing endpoints.

#### Configuration
The following configurations must be set in the `appsettings.json` file:
```json
"ConnectionStrings": {
  "ContactsDb": "Host=localhost;Port=5432;Database=ContactsDb;Username=postgres;Password=setur"
},
"MessagingSettings": {
  "Host": "amqp://localhost:5672",
  "Password": "guest",
  "Username": "guest"
},
"ContactsServiceClientConfig": {
  "ClientName": "PeopleServiceClient",
  "BaseUrl": "https://localhost:44300/",
  "PeopleStatisticsEndpoint": "statistics/people-count-by-locations"
}
```

### Api Gateway
- Response overwriting is not implemented thus URLs in the 201 created responses are wrong

---

## Getting Started
### Running with Docker
The easiest way to run the project is using Docker Compose. Run the following command:
```bash
docker-compose up --build
```
Once the services are running, visit `https://localhost:8080/swagger/index.html` to interact with the APIs. 
Make sure that all services are running.
If any services fail to start please try to restart the failed container. Services try to run db migrations on startup and they fail if Postgre is not ready

### Running Locally
Alternatively, you can run and debug the services individually via your IDE. Ensure PostgreSQL and RabbitMQ are running by starting the Docker Compose services first. Adjust connection strings and ports in the configuration files to match your local environment.

## Test Coverage
- Please take into consideration that this report includes projects that don't need tests in our case such as ApiGateway which lowers the total coverage percentage.
  
![2025-01-18_03h38_07](https://github.com/user-attachments/assets/8a458d28-9ffe-4b84-a486-ff016352a0bc)


---

Feel free to let me know if additional clarifications are needed!
