# Casino-microservices-app 🎰
A modular microservices architecture built in .NET, simulating a casino backend system.

## 🧱 Project Structure
.
├── AuthService/
├── WalletService/
├── LobbyService/
├── GameService/
├── docker-compose.yml
└── README.md


---

## 📦 Services

### 🔐 AuthService
- Handles user authentication and JWT issuance.
- Port: `5062`
- Exposed internally for inter-service authentication.

### 💰 WalletService
- Manages user balances, transactions, deposits, and withdrawals.
- Port: `5063`
- Depends on: `AuthService`

### 🧩 LobbyService
- Handles game tables, player lobbies, matchmaking, and room management.
- Port: `5064`
- Depends on: `AuthService`, `WalletService`

### 🎮 GameService
- Manages core game logic, game rounds, and results.
- Port: `5065`
- Depends on: `AuthService`

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker & Docker Compose](https://docs.docker.com/compose/install/)

---

## 🐳 Running the Project with Docker Compose

Run all services in development mode:

docker-compose up --build


## Front-end instructions

# Run these commands to run front-end service

cd casino-app-fe

npm run dev






