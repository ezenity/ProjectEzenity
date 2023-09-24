# Ezenity Backend API

## Introduction

The Ezenity Backend API serves as a versatile backend for any frontend technology that can plug into it. It offers a range of features from account creation and management to customizable profiles and sections. The API is designed to be flexible, allowing for various use-cases including basic portfolio frontends.

## Features

- **Account Management**: Create, update, and manage user accounts.
- **Customizable Profiles**: Future support for customizable user profiles.
- **Admin Features**: Ability to create custom email templates.
- **Logging**: Custom logging using Serilog.
- **Custom Sections**: Create custom sections and determine their population logic.
- **Email Templates**: Admin accounts can create and manage custom email templates.
- **Security**: Custom JWT for authorization, with plans to upgrade from SHA256 to SHA512.
- **API Documentation**: SwaggerUI for endpoint documentation.
- **Future Plans**: Payment processing feature to be added.

## Technologies Used

- **.NET 6.0**: The API is built using .NET 6.0.
- **Serilog**: Custom logging for better traceability, with daily log files.
- **Swashbuckle (Swagger)**: API documentation and testing.
- **Custom JWT**: Utilizes custom JWT for authorization. (Upgrading from SHA256 to SHA512 is in progress)

## Installation and Setup

> Note: Default email templates and an admin account for basic functions will be available in a future release.

## API Documentation

The API endpoints are documented using SwaggerUI. You can access it at `[Swagger UI URL here]`.

## Future Enhancements

- Implement a payment processing feature.
- Increase security from SHA256 to SHA512.

## Contributing

If you would like to contribute to this project, please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.


# Ezenity Frontend

## Introduction

The Ezenity Frontend serves two main purposes: to showcase the capabilities of the Ezenity Backend API and to act as a personal website. The project is in its early beta stages and is designed to be modular, configurable, and customizable.

## Features

- **Profile Creation**: Users can create and manage their profiles.
- **Role-Based Permissions**: Modify sections based on role ID.
- **Section Modification**: Users with the correct role ID can modify sections. (Note: This is in early beta and a lot of functionalities are hardcoded)
- **Modular Design**: Everything will be modular and configurable in the future.

## Technologies Used

- **React**: The frontend is built using React, but it can be adapted for other frontend technologies like Angular.

## Installation and Setup

> Future Goal: The frontend will eventually feature a setup wizard that guides users through the initial setup process.

## Usage Examples

> Note: No specific use-cases or examples are available at this moment.

## Contributing

If you would like to contribute to this project, please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

