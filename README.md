# Hospital Booking API

This is the backend API for the Hospital Booking Application, built with ASP.NET Core and following Clean Architecture principles.

## Features

- **Authentication & Authorization**: Secure JWT-based authentication.
- **Patient Management**: CRUD operations for patient records.
- **Doctor Management**: Manage specialized doctors and their schedules.
- **Appointment Booking**: Workflow for scheduling appointments.
- **Admin Dashboard**: Endpoints for managing departments and users.

## Technology Stack

- **Framework**: .NET 10 (Preview)
- **Database**: SQL Server (EF Core)
- **Documentation**: Scalar / OpenAPI

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or Express)

## Getting Started

1.  **Clone the repository**
2.  **Configure Database**
    Update `appsettings.json` in `HospitalBooking.API` with your connection string.
3.  **Apply Migrations**
    ```bash
    dotnet ef database update --project HospitalBooking.Infrastructure --startup-project HospitalBooking.API
    ```
4.  **Run the Application**
    ```bash
    dotnet run --project HospitalBooking.API
    ```
    The API will be available at `https://localhost:7196` (check launch logs for exact port).

## Project Structure

- **HospitalBooking.API**: Entry point, Controllers, Configuration.
- **HospitalBooking.Application**: Business logic, DTOs, Interfaces.
- **HospitalBooking.Domain**: Core entities and domain logic.
- **HospitalBooking.Infrastructure**: Database context, Repositories, External services.