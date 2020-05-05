# ODS Api WireMock

This docker project creates a mocked web server to support external interactions from the Identity application to the NHS ODS web service.

It uses the [.Net WireMock Docker Image](https://github.com/WireMock-Net/WireMock.Net-docker/) as a base. All endpoints are created and deleted as part of the integration tests.