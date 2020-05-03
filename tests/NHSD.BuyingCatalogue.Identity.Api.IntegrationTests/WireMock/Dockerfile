FROM sheyenrath/wiremock.net:1.1.6
COPY mappings/ /app/__admin/mappings/
WORKDIR /app
ENTRYPOINT ["./wiremock-net", "--Urls", "http://*:80"]
CMD ["--WireMockLogger", "WireMockConsoleLogger", "--ReadStaticMappings","True"]