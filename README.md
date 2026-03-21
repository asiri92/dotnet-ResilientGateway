# 🚀 Resilient API Gateway (ASP.NET Core Minimal API)

A production-inspired API Gateway built using ASP.NET Core Minimal APIs that demonstrates resilience engineering, observability, and distributed system patterns.

## 🧠 What This Project Demonstrates

- Retry, Timeout, Circuit Breaker
- Correlation ID propagation
- Structured logging (Serilog)
- Distributed tracing (OpenTelemetry)
- Health checks (Liveness vs Readiness)

## 🏗 Architecture Overview

Client → API Gateway → HttpClient Pipeline → Downstream Service

## ⚙️ Tech Stack

- .NET 8
- HttpClientFactory
- Microsoft Resilience Pipeline
- Serilog
- OpenTelemetry

## ❤️ Health Checks

- /health/live → App running
- /health/ready → Dependency check

## 🧪 How to Run

cd src/DownstreamService
dotnet run

cd src/ApiGateway
dotnet run
