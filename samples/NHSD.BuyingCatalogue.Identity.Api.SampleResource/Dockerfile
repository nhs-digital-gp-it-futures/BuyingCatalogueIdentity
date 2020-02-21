FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["samples/NHSD.BuyingCatalogue.Identity.Api.SampleResource/NHSD.BuyingCatalogue.Identity.Api.SampleResource.csproj", "samples/NHSD.BuyingCatalogue.Identity.Api.SampleResource/"]
RUN dotnet restore "samples/NHSD.BuyingCatalogue.Identity.Api.SampleResource/NHSD.BuyingCatalogue.Identity.Api.SampleResource.csproj"
COPY . .
WORKDIR "/src/samples/NHSD.BuyingCatalogue.Identity.Api.SampleResource"
RUN dotnet build "NHSD.BuyingCatalogue.Identity.Api.SampleResource.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.BuyingCatalogue.Identity.Api.SampleResource.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHSD.BuyingCatalogue.Identity.Api.SampleResource.dll"]