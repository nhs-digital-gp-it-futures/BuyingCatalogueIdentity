FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
RUN apk update && apk add icu-libs iproute2
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /
COPY ["src/NHSD.BuyingCatalogue.Identity.Common/NHSD.BuyingCatalogue.Identity.Common.csproj", "src/NHSD.BuyingCatalogue.Identity.Common/"]
COPY ["src/NHSD.BuyingCatalogue.Organisations.Api/NHSD.BuyingCatalogue.Organisations.Api.csproj", "src/NHSD.BuyingCatalogue.Organisations.Api/"]
RUN dotnet restore "src/NHSD.BuyingCatalogue.Organisations.Api/NHSD.BuyingCatalogue.Organisations.Api.csproj"

COPY . .
WORKDIR "/src/NHSD.BuyingCatalogue.Organisations.Api"
RUN dotnet build "NHSD.BuyingCatalogue.Organisations.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.BuyingCatalogue.Organisations.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHSD.BuyingCatalogue.Organisations.Api.dll"]
