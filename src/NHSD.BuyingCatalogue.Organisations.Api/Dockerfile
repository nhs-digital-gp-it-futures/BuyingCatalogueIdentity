#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /
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
RUN apt update && apt install iproute2 -y 
ENTRYPOINT ["dotnet", "NHSD.BuyingCatalogue.Organisations.Api.dll"]