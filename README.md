## Table of Contents
- [Project Ezenity](#project-ezenity)
  - [Introduction](#introduction)
  - [Architecture](#architecture)
  - [Features](#features)
  - [Technologies Used](#technologies-used)
  - [Security](#security)
    - [SHA-512 Encryption](#sha-512-encryption)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Database Migrations](#database-migrations)
  - [Configuration](#configuration)
  - [Usage](#usage)
    - [API Documentation](#api-documentation)
    - [Token Handling](#token-handling)
    - [Database Operations](#database-operations)
    - [Logging](#logging)
    - [Testing](#testing)
  - [Future Enhancements](#future-enhancements)
- [Frontend](#frontend)
  - [Introduction](#introduction-1)
  - [Features](#features-1)
  - [Technologies Used](#technologies-used-1)
  - [Installation](#installation-1)
  - [Configuration](#configuration-1)
  - [Usage](#usage-1)
- [Contributing](#contributing)
- [License](#license)
---

# Project Ezenity

### Introduction

The Ezenity.API serves as a versatile backend for any frontend technology that can plug into it. It offers a range of features from account creation and management to customizable profiles and sections. The API is designed to be flexible, allowing for various use-cases including basic portfolio frontends.

### Architecture

The backend is organized into multiple class libraries, following the Clean Architecture pattern:

- `Ezenity.API`: The API layer that exposes endpoints.
- `Ezenity.Core`: Contains entities and core business logic.
- `Ezenity.Infrastructure`: Houses the data access logic and other infrastructural concerns.
- `Ezenity.DTOs`: Data Transfer Objects for communication between layers.

The solution is housed under the name `ProjectEzenity` (Previously known as `Ezenity_VPN_Server`).

### Features

- **Account Management**: Create, update, and manage user accounts.
- **Customizable Profiles**: Future support for customizable user profiles.
- **Admin Features**: Ability to create custom email templates.
- **Logging**: Custom logging using Serilog.
- **Custom Sections**: Create custom sections and determine their population logic.
- **Email Templates**: Admin accounts can create and manage custom email templates.
- **Security**: Custom JWT for authorization, with plans to upgrade from SHA256 to SHA512.
- **API Documentation**: SwaggerUI for endpoint documentation.
- **Future Plans**: Payment processing feature to be added.

### Technologies Used

- **.NET 6.0**: The API is built using .NET 6.0.
- **Serilog**: Custom logging for better traceability, with daily log files.
- **Swashbuckle (Swagger)**: API documentation and testing.
- **Custom JWT**: Utilizes custom JWT for authorization. (Upgrading from SHA256 to SHA512 is in progress)

### Security

#### SHA-512 Encryption
The application uses SHA-512 for enhanced security in JWT token handling and other cryptographic operations.

---

### Prerequisites
- .NET SDK 6.0 or higher
- SQL Server (or a compatible database engine)
- Your preferred API testing tool (e.g., Postman)

### Installation

> Note: Default email templates and an admin account for basic functions will be available in a future release.

1. Clone the repository:
    ```bash
    git clone https://github.com/ezenity/ProjectEzenity.git
    ```
2. Navigate to the `ProjectEzenity` folder:
    ```bash
    cd ProjectEzenity/Ezenity.API
    ```
3. Restore the NuGet packages:
    ```bash
    dotnet restore
    ```
4. Build the project:
    ```bash
    dotnet build
    ```
5. Run the project:
    ```bash
    dotnet run
    ```
 
### Database Migrations 

1. Navigate to Ezenity.API Project
    ```bash
    cd ProjectEzenity/Ezenity.API
    ```
2. Run the following command to create a new migration:
    ```bash
    dotnet ef migrations add InitialCreate --startup-project ../Ezenity.API --project ../Ezenity.Infrastructure --namespace Ezenity.Infrastructure.Data.Migrations
    ```
3. To apply the migration and update the database, run:
    ```bash
    dotnet ef database update --startup-project ../Ezenity.API --project ../Ezenity.Infrastructure
    ```

### Configuration
- **AppSettings**: Description of the properties in `appsettings.json` and their functions.
- **Environment Variables**: You'll want to setup a **SECRET_KEY** or a file containing the Secret Key.

---

### Usage

#### API Documentation

The API endpoints are documented using SwaggerUI. You can access it at `http://localhost:5001`.

#### Token Handling
> Note: No specific Token Handling are available at this moment.

#### Database Operations
> Note: No specific Database Operations are available at this moment.

#### Logging
> Note: No specific Logging are available at this moment.

#### Testing
> Note: No specific Testing are available at this moment.

---

### Future Enhancements

- Implement a payment processing feature.
- Increase security from SHA256 to SHA512.



# Frontend

### Introduction

The Ezenity Frontend serves two main purposes:
- to showcase the capabilities of the Ezenity Backend API
- To act as a personal website.

The project is in its early beta stages and is designed to be modular, configurable, and customizable. Future versions will include `Ezenity.React`, `Ezenity.Angular`, and potentially other frontend technologies.

### Features

- **Profile Creation**: Users can create and manage their profiles.
- **Role-Based Permissions**: Modify sections based on role ID.
- **Section Modification**: Users with the correct role ID can modify sections. (Note: This is in early beta and a lot of functionalities are hardcoded)
- **Modular Design**: Everything will be modular and configurable in the future.

### Technologies Used

- **React**: The frontend is built using React, but it can be adapted for other frontend technologies like Angular.

### Installation

> Future Goal: The frontend will eventually feature a setup wizard that guides users through the initial setup process.

### Configuration

> Note: No specific configuration are available at this moment.

### Usage

> Note: No specific use-cases or examples are available at this moment.



# Contributing

If you would like to contribute to this project, please fork the repository and submit a pull request.

# License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

