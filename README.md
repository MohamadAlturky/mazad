# Mazad Project

## Overview

The Mazad project is a comprehensive application designed to manage auctions. It consists of several components, each serving a specific purpose within the application. The project is structured into different modules, including Admin, API, Use Cases, and Core.

## Project Structure

- **Mazad.Admin**: This module contains the front-end code for the admin panel. It is built using modern web technologies and includes configuration files for TypeScript, Vite, Tailwind CSS, and ESLint.
  
- **Mazad.Api**: This module serves as the backend API for the application. It is built using .NET and includes configuration files for app settings and HTTP requests.

- **Mazad.UseCases**: This module contains the business logic of the application. It includes various use cases that define the operations and processes within the application.

- **Mazad.Core**: This module contains the core domain models and shared resources used across the application. It also includes database migrations.

## Getting Started

### Prerequisites

- Node.js and npm for the front-end development.
- .NET SDK for the backend development.

### Installation

1. Clone the repository to your local machine.
2. Navigate to the `Mazad.Admin` directory and run `npm install` to install the front-end dependencies.
3. Navigate to the `Mazad.Api` directory and restore the .NET packages using `dotnet restore`.

### Running the Application

- To start the admin panel, navigate to the `Mazad.Admin` directory and run `npm run dev`.
- To start the API, navigate to the `Mazad.Api` directory and run `dotnet run`.

## Configuration

- The application settings for the API are located in `appsettings.json` and `appsettings.Development.json`.
- The front-end configuration files include `vite.config.ts`, `tsconfig.json`, and `tailwind.config.ts`.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request for any improvements or bug fixes.

## License

This project is licensed under the MIT License. 