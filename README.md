# Project Backend

## Table of Contents

1. [Getting Started](#getting-started)
   - [Prerequisites](#prerequisites)
   - [Clone the Repository](#clone-the-repository)
   - [Configure AppSettings](#configure-appsettings)
   - [Testing](#testing)
2. [Accessing the API](#accessing-the-api)

This repository contains the backend code for the BookStore. It provides RESTful APIs for managing books and authors.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 7.0 or higher)
- [Docker](https://www.docker.com/get-started) (for local development and testing)

### Clone the Repository

To get started, clone the repository to your local machine:

```bash
https://github.com/kyriakosgeorgiades/Store.git
 ```

Change into the project folder:
```bash
cd Store
```

### Configure AppSettings

Before running the application, make sure to update the `AppSettings.Development.json` file with your local development settings:
For the sake of the exercises, the user id and password are in the settings in the repository ready to be used.

1. Change the `ServerName` property to "localhost" for Entity Framework migration and integration testing to work correctly:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourDatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
}
```

2. To complete the migration start the containers which we be auto promt or use ```docker-compose up --build``` switch the ServerName to "localhost" for the application to connect to the database. To update the database run you need to switch the startup project to "Store" and then run
```bash
Update-Database
```

A seeder is applied to the database each time the docker starts up for the sake of having ready-to-use data for demo and faster development.

3. After is completed switch back the server to "db" and switch the startup project to docker-compose to run the API with the database.
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db;Database=YourDatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
}
```

### Testing
To run the tests both integration and unit test switch the connection string to "localhost" and right-click on the Test project and select "Run Tests". After you are done don't forget to switch back to "db" the server in the connection string


### Accessing the API
Once the application is running, you can access the API documentation at:

http://localhost:8081/swagger/index.html

All book routes except the search books endpoint, require authorization, so make sure to obtain a Bearer token before testing them in the Swagger interface.

