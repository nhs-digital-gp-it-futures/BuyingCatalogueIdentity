# Identity Api WireMock

This docker project creates a mocked web server to support external interactions from the Identity application to a dummy Public Browse application.

It uses the [.Net WireMock Docker Image](https://github.com/WireMock-Net/WireMock.Net-docker/) as a base, and then overrides it to provide default routes and responses as below, which can then be used to mimic any external services e.g. Public Browse Homepage

- /PublicBrowse

The mappings are set in [default-mapping.json](mappings/default-mapping.json).