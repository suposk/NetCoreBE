# NetCoreBE

This is a sample .NET Rest API Application using Clean Architecture.

## Features

- **.NET 8**: Utilizes the latest version of .NET for building the application.
- **Docker**: Containerizes the application for easy deployment and scalability.
- **Clean Architecture**: Implements the Clean Architecture principles, organizing the project into Domain, Application, Infrastructure, and API layers.
- **Unit Tests**: Includes unit tests to ensure code quality and reliability.

## Project Structure

```
NetCoreBE
├── Domain          # Core business logic and entities
├── Application     # Application services and use cases
├── Infrastructure  # Data access and external service implementations
└── Api             # Presentation layer (controllers, API endpoints)
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)

### Running the Application

1. Clone the repository:
   ```sh
   git clone https://github.com/suposk/NetCoreBE.git
   cd NetCoreBE
   ```

2. Build and run the application using Docker:
   ```sh
   docker-compose up --build
   ```

3. The API should now be running and accessible at `https://localhost:6006/`.

### Running Tests

To run the unit tests, use the following command:
```sh
dotnet test
```

### Summary of Docs Files

**TicketV1.http**
- Contains HTTP requests for managing tickets in version 1 of the API.
- Includes GET, POST, PUT, DELETE requests for various endpoints such as `/Ticket/`, `/Ticket/{{id}}`, `/Ticket/Search`, and `/Ticket/Seed/{number}`.

**TicketV2.http**
- Contains HTTP requests for managing tickets in version 2 of the API.
- Similar to TicketV1.http but adapted for API version 2.
- Includes GET, POST, PUT, DELETE requests for various endpoints such as `/Ticket/`, `/Ticket/{{id}}`, `/Ticket/Search`, and `/Ticket/Seed/{number}`.

**CrudExample.http**
- Contains HTTP requests for managing CRUD examples.
- Includes GET, POST, PUT, DELETE requests for various endpoints such as `/CrudExample/`, `/CrudExample/{{id}}`, and `/CrudExample/Seed/{number}`.


## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more details.

---

Feel free to modify this README as needed to better fit your project's specifics.
