# Offshore Wind Farm Monitoring System

## Overview

This project is a full-stack web application for monitoring and controlling offshore wind turbines in real time.

The system connects to a wind turbine simulator and allows inspectors to see live telemetry data, visualize metrics in charts, receive alerts, and send control commands to turbines through a web interface.

Front-end link:
https://fullstackiot-web.fly.dev/

Back-end link:
https://fullstackiot-server.fly.dev/

Simulator used in the project:  
https://sea-fullstack.web.app/

---

## What the System Does

The application allows a windmill inspector to:

- view real-time turbine telemetry
- see graphs showing turbine metrics over time
- receive alerts when something unexpected happens
- control turbine settings from a web interface
- authenticate before sending commands
- keep a history of all operator actions

All telemetry data, alerts, and commands are stored in a relational database so the system maintains a full history of turbine activity.

---

## How the Solution Works

The backend subscribes to turbine data using **MQTT** and processes the incoming telemetry.  
All metrics and alerts are saved to a **PostgreSQL database** using **Entity Framework Core**.

To provide live updates to the frontend, the system uses **Server-Sent Events (SSE)** through the **StateleSSE.AspNetCore** library. This allows the UI to receive real-time data without polling.

The React frontend displays turbine telemetry in charts, shows alerts, and allows authenticated operators to send control commands to turbines.

---

## Technologies Used

### Backend
- .NET 10
- C#
- ASP.NET Core Web API
- StateleSSE.AspNetCore
- Mqtt.Controllers
- Entity Framework Core
- PostgreSQL
- Redis
- JWT Authentication
- BCrypt password hashing
- NSwag
- `.env` configuration

### Frontend
- React
- TypeScript
- Vite
- TailwindCSS
- Recharts
- Lucide

### DevOps / Deployment
- Fly.io deployment
- `fly.toml` configuration
- CI pipeline

---

## Project Structure

- `server/api` – ASP.NET Core API, MQTT integration, SSE, authentication, command handling
- `server/dataaccess` – Entity Framework Core data layer and database models
- `client` – React frontend for monitoring, charts, alerts, and controls

---

## Deployment

The application is deployed and running online using **Fly.io**, fulfilling the requirement of a live running system.

---

## Summary

This project demonstrates how real-time IoT data from wind turbines can be collected, processed, stored, and visualized in a modern full-stack application using .NET and React.

---

## Created by ELK

Ali Emre Uzunoglu
Katja Tamstrup Strunck
Laura Shpakova