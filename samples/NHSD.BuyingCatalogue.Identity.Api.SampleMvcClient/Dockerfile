#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt update && apt install iproute2 libnss3-tools wget -y
RUN wget https://github.com/FiloSottile/mkcert/releases/download/v1.4.3/mkcert-v1.4.3-linux-amd64 && \
    mv mkcert-v1.4.3-linux-amd64 mkcert && chmod +x mkcert
RUN cp mkcert /usr/local/bin/
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["samples/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.csproj", "samples/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient/"]
RUN dotnet restore "samples/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.csproj"
COPY . .
WORKDIR "/src/samples/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient"
RUN dotnet build "NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/build/start.sh .
RUN ["chmod", "+x", "./start.sh"]
ENTRYPOINT ["/bin/bash", "-c",  "./start.sh"]
