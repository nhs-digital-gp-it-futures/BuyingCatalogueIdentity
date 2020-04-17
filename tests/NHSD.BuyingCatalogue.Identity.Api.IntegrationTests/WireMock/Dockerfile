FROM sheyenrath/wiremock.net:1.1.6
WORKDIR /app
ENTRYPOINT ["./wiremock-net", "--Urls", "http://*:80"]
CMD ["--WireMockLogger", "WireMockConsoleLogger"]
