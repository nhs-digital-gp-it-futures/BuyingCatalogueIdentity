FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
RUN apk update && apk add icu-libs iproute2
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM node:14 AS pre-build
WORKDIR /node
COPY src/NHSD.BuyingCatalogue.Identity.Api/package.json .
COPY src/NHSD.BuyingCatalogue.Identity.Api/Styles ./Styles
RUN npm install && npm run build

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /
COPY ["src/NHSD.BuyingCatalogue.Identity.Common/NHSD.BuyingCatalogue.Identity.Common.csproj", "src/NHSD.BuyingCatalogue.Identity.Common/"]
COPY ["src/NHSD.BuyingCatalogue.Identity.Api/NHSD.BuyingCatalogue.Identity.Api.csproj", "src/NHSD.BuyingCatalogue.Identity.Api/"]
COPY ["NuGet.config", "."]
RUN dotnet restore "src/NHSD.BuyingCatalogue.Identity.Api/NHSD.BuyingCatalogue.Identity.Api.csproj"

COPY . .
WORKDIR "/src/NHSD.BuyingCatalogue.Identity.Api"

COPY --from=pre-build /node/wwwroot/css wwwroot/css
RUN dotnet build "NHSD.BuyingCatalogue.Identity.Api.csproj" --no-restore -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.BuyingCatalogue.Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/build/start.sh .
RUN ["chmod", "+x", "./start.sh"]
ENTRYPOINT ["sh", "./start.sh"]
