# Buying Catalogue Identity - Authentication & authorization service for the NHS Digital Buying Catalogue

## IMPORTANT NOTES

**You can use either the latest version of Visual Studio or .NET CLI for Windows, Mac and Linux**.

### Requirements

- .NET Version 5.0
- Docker
- node.js Version 14

> Before you begin please install **.NET 5.0**, **node.js 14** & **Docker** on your machine.

## Overview

This application uses **.NET 5** to provide an identity service currently implemented with IdentityServer 4 wrapping ASP.NET Identity. It is used to provide token-based authentication and API access control in the associated Buying Catalogue services.

### Project structure

This repository uses **.NET 5**, **node.js** and **Docker**.

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

This server is [maildev](https://github.com/maildev/maildev) server with [stunnel](https://www.stunnel.org/) wrapper around it for TLS 1.2 support.

##### How to connect

| From                       | Host                       | Port  | TLS |
|            :-:             |            :-:             |  :-:  | :-: |
| within the docker network  | nhsd.buyingcatalogue.email | 25    |  X  |
| within the docker network  | nhsd.buyingcatalogue.email | 587   | 1.2 |
| outside the docker network | localhost                  | 1025  |  X  |
| outside the docker network | localhost                  | 1587  | 1.2 |

Navigate to [localhost:1180/email](http://localhost:1180/email) to view the mailbox UI.

## Running the Application

### Prerequisites

Before running the application it is necessary to generate a certificate. Scripts are provided (details below) that will create the required certificate. These scripts should only need to be run once prior to starting the application or running the tests for the first time. The script will create a directory named `mkcert`; do not delete this directory – it is required for HTTPS support and for the integration tests. However, if any issues occur or the `mkcert` directory is accidentally deleted the script can be re-run.

#### Windows

Run the `Create Certificate.ps1` PowerShell script.

#### Linux

Run the `create_certificate.sh` bash script.

**Note:** this script assumes you are running a Debian-based distribution. If you are running a distribution that does not use APT for package management you will need to edit the script or run the equivalent set of commands manually.

### To start the application

To start up the web application, run the following command from the root directory of the repository.

```shell
docker-compose up -d --build
```

This will start the application in a docker container. You can verify that the service has launched correctly by navigating to the following url via any web browser.

```http
https://localhost:9070/identity/account/login
```

### To stop the application

To stop the application, run the following command from the root directory of the repository, which will stop the service within the docker container.

```shell
docker-compose down -v
```

### Running the Integration Tests

Start the application as decribed in [running the application](#running-the-application) and run the integration tests using your preferred test runner.
