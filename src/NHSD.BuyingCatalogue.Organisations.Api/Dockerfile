FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt update && apt install iproute2 -y 
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
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
