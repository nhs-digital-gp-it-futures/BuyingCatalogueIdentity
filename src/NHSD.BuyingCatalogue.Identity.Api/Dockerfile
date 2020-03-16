FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/NHSD.BuyingCatalogue.Identity.Api/NHSD.BuyingCatalogue.Identity.Api.csproj", "src/NHSD.BuyingCatalogue.Identity.Api/"]
RUN dotnet restore "src/NHSD.BuyingCatalogue.Identity.Api/NHSD.BuyingCatalogue.Identity.Api.csproj"

RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_12.x | bash - && \
    apt-get install -y nodejs

COPY . .
WORKDIR "/src/src/NHSD.BuyingCatalogue.Identity.Api"
RUN dotnet build "NHSD.BuyingCatalogue.Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.BuyingCatalogue.Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt update && apt install iproute2 -y 
ENTRYPOINT ["dotnet", "NHSD.BuyingCatalogue.Identity.Api.dll"]