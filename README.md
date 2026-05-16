# AI-Assisted Microservice Incident Monitoring & Root Cause Analysis Platform

A cloud-native AI-powered observability platform for distributed microservice environments.

## Features

- Real-time log ingestion
- Event-driven architecture with RabbitMQ
- AI-powered root cause analysis
- Redis caching
- PostgreSQL persistence
- Dockerized microservices
- FastAPI AI inference service
- React monitoring dashboard
- API Gateway using YARP

## Architecture

Frontend (React)
↓
API Gateway (YARP)
↓
Microservices (.NET 8)
1. LogService
2. IncidentService
3. AIInferenceService (FastAPI + LLM)

Infrastructure
1. RabbitMQ
2. PostgreSQL
3. Redis
4. Docker

## Technologies

- ASP.NET Core 8
- RabbitMQ
- PostgreSQL
- Redis
- Docker
- FastAPI
- React
- Gemini
- YARP API Gateway

## Run Project

```bash
docker compose up --build
