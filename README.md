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

## Documentation
TicketV1.http
TicketV2.http
CrudExample.http


## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more details.

---

Feel free to modify this README as needed to better fit your project's specifics.
