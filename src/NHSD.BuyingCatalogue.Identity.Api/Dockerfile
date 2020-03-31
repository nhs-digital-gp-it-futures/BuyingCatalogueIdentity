FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt update && apt install iproute2 -y 
WORKDIR /app
EXPOSE 80

FROM node:12 AS pre-build
WORKDIR /node
COPY src/NHSD.BuyingCatalogue.Identity.Api/package.json .
COPY src/NHSD.BuyingCatalogue.Identity.Api/Styles ./Styles
RUN npm install && npm run build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /
COPY ["src/NHSD.BuyingCatalogue.Identity.Common/NHSD.BuyingCatalogue.Identity.Common.csproj", "src/NHSD.BuyingCatalogue.Identity.Common/"]
COPY ["src/NHSD.BuyingCatalogue.Identity.Api/NHSD.BuyingCatalogue.Identity.Api.csproj", "src/NHSD.BuyingCatalogue.Identity.Api/"]
RUN dotnet restore "src/NHSD.BuyingCatalogue.Identity.Api/NHSD.BuyingCatalogue.Identity.Api.csproj"

COPY . .
WORKDIR "/src/NHSD.BuyingCatalogue.Identity.Api"

COPY --from=pre-build /node/wwwroot/css wwwroot/css
RUN dotnet build "NHSD.BuyingCatalogue.Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.BuyingCatalogue.Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHSD.BuyingCatalogue.Identity.Api.dll"]