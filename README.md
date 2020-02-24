# BuyingCatalogueIdentity - Service architecture for the NHS Digital Buying Catalogue Identity .Net Core application

## IMPORTANT NOTES!
**You can use either the latest version of Visual Studio or .NET CLI for Windows, Mac and Linux**.

### Architecture overview
This application uses **.NET core** to provide an API capable of runnning Linux or Windows.

### Overview of the application code
This repository uses **.NET Core** and **Docker**.

It contains one endpoint

- account/login?returnUrl=
  - Returns a HTML view.

The application is broken down into the following project libraries:

- API 
  - Defines and exposes the available Buying Catalogue Identity endpoints.
- API.UnitTests
  - Contains all of the unit tests for the API project.
- API.IntegrationTests
  - Contains all of the integration tests for the API project.

## Setting up your development enviroment for the Buying Catalogue Identity

### Requirements

- .NET Core Version 3.1
- Docker

> Before you begin please install **.NET Core 3.1** & **Docker** on your machine.

## Running the API

### Running
To start up the API, run the command from the root directory of the repository.

```bash
docker-compose up -d --build
```

This will start the API in a docker container.

You can verify that the API has launched correctly by navigating to the following url via any web browser, using the following test data. This should successfully log in a user and bring back a return url.

Navigate to **http://localhost:8070/account/login**. 

**Email Address** - Michael

**Password** - Michael

### Stopping 
To stop the API, run the following command from the root directory of the repository, which will stop the API docker container.

```bash
docker-compose down -v
```

### Running the Integration Tests
TO BE COMPLETED