FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt update && apt install iproute2 -y 

FROM node:12 AS node-build
WORKDIR /node
COPY src/NHSD.BuyingCatalogue.Identity.Api/package.json .
COPY src/NHSD.BuyingCatalogue.Identity.Api/Styles ./Styles
RUN npm install && npm run build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /
COPY "NHSD.BuyingCatalogue.Identity-ExludeDatabase.sln" "NHSD.BuyingCatalogue.Identity-ExludeDatabase.sln"
COPY "samples/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.csproj" "samples/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient/NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.csproj"
COPY "samples/NHSD.BuyingCatalogue.Identity.Api.SampleResource/NHSD.BuyingCatalogue.Identity.Api.SampleResource.csproj" "samples/NHSD.BuyingCatalogue.Identity.Api.SampleResource/NHSD.BuyingCatalogue.Identity.Api.SampleResource.csproj"
COPY "samples/NHSD.BuyingCatalogue.Identity.Api.SampleClient/NHSD.BuyingCatalogue.Identity.Api.SampleClient.csproj" "samples/NHSD.BuyingCatalogue.Identity.Api.SampleClient/NHSD.BuyingCatalogue.Identity.Api.SampleClient.csproj"
COPY "src/NHSD.BuyingCatalogue.Organisations.Api/NHSD.BuyingCatalogue.Organisations.Api.csproj" "src/NHSD.BuyingCatalogue.Organisations.Api/NHSD.BuyingCatalogue.Organisations.Api.csproj"
COPY "tests/NHSD.BuyingCatalogue.Identity.Api.UnitTests/NHSD.BuyingCatalogue.Identity.Api.UnitTests.csproj" "tests/NHSD.BuyingCatalogue.Identity.Api.UnitTests/NHSD.BuyingCatalogue.Identity.Api.UnitTests.csproj"
COPY "src/NHSD.BuyingCatalogue.Identity.Common/NHSD.BuyingCatalogue.Identity.Common.csproj" "src/NHSD.BuyingCatalogue.Identity.Common/NHSD.BuyingCatalogue.Identity.Common.csproj"
COPY "src/NHSD.BuyingCatalogue.Identity.Api/NHSD.BuyingCatalogue.Identity.Api.csproj" "src/NHSD.BuyingCatalogue.Identity.Api/NHSD.BuyingCatalogue.Identity.Api.csproj"
COPY "tests/NHSD.BuyingCatalogue.Identity.Api.IntegrationTests/NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.csproj" "tests/NHSD.BuyingCatalogue.Identity.Api.IntegrationTests/NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.csproj"
COPY "tests/NHSD.BuyingCatalogue.Organisations.Api.UnitTests/NHSD.BuyingCatalogue.Organisations.Api.UnitTests.csproj" "tests/NHSD.BuyingCatalogue.Organisations.Api.UnitTests/NHSD.BuyingCatalogue.Organisations.Api.UnitTests.csproj"
COPY "tests/NHSD.BuyingCatalogue.Identity.Api.Testing.Data/NHSD.BuyingCatalogue.Identity.Api.Testing.Data.csproj" "tests/NHSD.BuyingCatalogue.Identity.Api.Testing.Data/NHSD.BuyingCatalogue.Identity.Api.Testing.Data.csproj"
RUN dotnet restore "NHSD.BuyingCatalogue.Identity-ExludeDatabase.sln"

COPY . .
COPY --from=node-build /node/wwwroot/css wwwroot/css
WORKDIR "/src/NHSD.BuyingCatalogue.Identity.Api"
RUN dotnet publish --no-restore -c Release -o /app

FROM build as publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN apt update && apt install iproute2 -y 
ENTRYPOINT ["dotnet", "NHSD.BuyingCatalogue.Identity.Api.dll"]