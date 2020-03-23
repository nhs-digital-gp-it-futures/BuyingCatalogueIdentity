# Buying Catalogue Identity - Authentication & authorization service for the NHS Digital Buying Catalogue

## IMPORTANT NOTES

**You can use either the latest version of Visual Studio or .NET CLI for Windows, Mac and Linux**.

### Requirements

- .NET Core Version 3.1
- Docker
- Nodejs Version 12.16.1

> Before you begin please install **.NET Core 3.1**, **Nodejs 12.16.1** & **Docker** on your machine.

## Overview

This application uses **.NET core** to provide an identity service currently implemented with IdentityServer 4 wrapping ASP.NET Identity. It is used to provide token-based authentication and API access control in the Buying Catalogue associated services.

### Project structure

This repository uses **.NET Core**, **Nodejs** and **Docker**.

It contains the following endpoints:

- account/login
  - Returns a HTML view.
- account/logout
  - Ends the current session
- api/v1/organisations
  - Returns all organisations
- api/v1/organisations/{id}
  - Returns the organisation with the given ID
- api/v1/organisations/{organisationId}/users
  - Returns the users for the organisation with the given ID

The application is broken down into the following project libraries:

- API
  - Defines and exposes the available Buying Catalogue Identity endpoints.
- API.UnitTests
  - Contains all of the unit tests for the API project.
- API.IntegrationTests
  - Contains all of the integration tests for the API project.
- Identity.UserDatabase
  - Defines the artefacts for the Identity database, which is used to manage users and organizations
- Organisations.API
  - Defines and exposes the endpoints for viewing and managing organisations and their users

#### Database project

The database project is a SQL Server project, which is only fully supported by Visual Studio on Windows. However, some limited functionality should still be available in other editors.

When making changes to the database make sure to remove the Docker volume as described [below](#to-stop-the-application) before [running the application](#running-the-application).

#### STMP Server

A local SMTP server has been added to the docker network for development purposes.

##### How to connect

| From                       | Host                       | Port  |
|            :-:             |            :-:             |  :-:  |
| within the docker network  | nhsd.buyingcatalogue.email | 25    |
| outside the docker network | localhost                  | 1025  |

Navigate yourself to [localhost:1080](http://localhost:1080/) to view the mailbox UI

## Running the Application

To start up the web application, run the following command from the root directory of the repository.

```shell
docker-compose up -d --build
```

This will start the application in a docker container. You can verify that the service has launched correctly by navigating to the following url via any web browser.

```http
http://localhost:8070/account/login
```

### To stop the application

To stop the application, run the following command from the root directory of the repository, which will stop the service within the docker container.

```shell
docker-compose down -v
```

### Running the Integration Tests

Start the application as decribed in [running the application](#running-the-application) and run the integration tests using your preferred test runner.
