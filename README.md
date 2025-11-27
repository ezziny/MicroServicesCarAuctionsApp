# MicroServices Car Auctions App

A full-stack microservices-based car auction platform. This solution is built with ASP.NET Core, MassTransit, MongoDB, PostgreSQL, RabbitMQ, and Next.js (frontend coming soon!).

## Table of Contents
- [Architecture Overview](#architecture-overview)
- [Microservices](#microservices)
  - [AuctionService](#auctionservice)
  - [BiddingService](#biddingservice)
  - [SearchService](#searchservice)
  - [IdentityService](#identityservice)
  - [NotificationService](#notificationservice)
  - [GatewayService](#gatewayservice)
- [Contracts](#contracts)
- [Frontend](#frontend)
- [Development & Running](#development--running)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Contributing](#contributing)

---

## Architecture Overview
This solution follows a microservices architecture. Each service is independently deployable, communicates via HTTP/gRPC and RabbitMQ, and persists its own data. The system is orchestrated using Docker Compose for local development.


## Microservices

### AuctionService
- Manages car auctions: create, update, finish, delete.
- Exposes REST API and gRPC endpoints.
- Persists auctions in PostgreSQL.
- Publishes auction events to RabbitMQ.
- Directory: `src/AuctionService`

### BiddingService
- Handles bids on auctions.
- Validates bids, enforces auction rules.
- Persists bids in MongoDB.
- Consumes auction events from RabbitMQ.
- Directory: `src/BiddingService`

### SearchService
- Provides search capabilities for auctions and bids.
- Uses MongoDB for fast queries.
- Consumes events to keep search index up-to-date.
- Directory: `src/SearchService`

### IdentityService
- Manages user authentication and authorization.
- Uses ASP.NET Identity and PostgreSQL.
- Issues JWT tokens for secure API access.
- Directory: `src/IdentityService`

### NotificationService
- Sends notifications to users (email, websocket, etc.).
- Listens for auction and bid events via RabbitMQ.
- Directory: `src/NotificationService`

### GatewayService
- API gateway for routing requests to backend services.
- Handles authentication, request forwarding, and aggregation.
- Directory: `src/GatewayService`

## Contracts
- Shared message contracts for inter-service communication (events, commands).
- Used by MassTransit and gRPC.
- Directory: `src/Contracts`

## Frontend
- **Coming soon!**
- Next.js app for user-facing web experience.
- Directory: `frontend/web-app`

## Development & Running

### Prerequisites
- Docker & Docker Compose
- .NET 9 SDK
- Node.js (for frontend)

### Running Locally
1. Clone the repo:
   ```bash
   git clone https://github.com/ezziny/MicroServicesCarAuctionsApp.git
   cd MicroServicesCarAuctionsApp
   ```
2. Build and start all services:
   ```bash
   docker compose up --build
   ```
3. Access services:
   - AuctionService: [http://localhost:7001](http://localhost:7001)
   - BiddingService: [http://localhost:7003](http://localhost:7003)
   - SearchService: [http://localhost:7002](http://localhost:7002)
   - IdentityService: [http://localhost:5001](http://localhost:5001)
   - NotificationService: [http://localhost:7004](http://localhost:7004)
   - GatewayService: [http://localhost:5279](http://localhost:5279)
   - Frontend (when available): [http://localhost:3000](http://localhost:3000)

### Environment Variables
See `docker-compose.yml` for all environment variables and service configuration.

## Project Structure
```
MicroservicesApp/
├── docker-compose.yml
├── MicroservicesApp.sln
├── frontend/
│   └── web-app/         # Next.js frontend (coming soon)
├── src/
│   ├── AuctionService/  # Auction microservice
│   ├── BiddingService/  # Bidding microservice
│   ├── SearchService/   # Search microservice
│   ├── IdentityService/ # Identity/auth microservice
│   ├── NotificationService/ # Notification microservice
│   ├── GatewayService/  # API gateway
│   └── Contracts/       # Shared message contracts
```

## Tech Stack
- **Backend:** ASP.NET Core, MassTransit, gRPC
- **Databases:** PostgreSQL, MongoDB
- **Messaging:** RabbitMQ
- **Frontend:** Next.js (React) *(coming soon)*
- **Containerization:** Docker, Docker Compose

## Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](LICENSE)

---

*This project is under active development. Frontend will be available soon. Star the repo to follow updates!*