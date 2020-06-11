FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS dacpacbuild
WORKDIR ~/dacpac
COPY src/NHSD.BuyingCatalogue.Identity.UserDatabase.Deployment/*.csproj .
COPY src/NHSD.BuyingCatalogue.Identity.UserDatabase .
RUN rm -rf Deployment \
    && dotnet build "NHSD.BuyingCatalogue.Identity.UserDatabase.Deployment.csproj" -c Release -o build

FROM mcr.microsoft.com/mssql-tools:latest AS dacfx
RUN apt-get update && apt-get install libunwind8 libicu-dev wget unzip -y
WORKDIR ~
RUN wget https://go.microsoft.com/fwlink/?linkid=2128144 -O sqlpackage.zip \
    && mkdir sqlpackage \
    && unzip sqlpackage.zip -d /sqlpackage \
    && chmod a+x /sqlpackage/sqlpackage
COPY --from=dacpacbuild ~/dacpac/build/*.dacpac .
COPY src/NHSD.BuyingCatalogue.Identity.UserDatabase.Deployment/entrypoint.sh .
COPY src/NHSD.BuyingCatalogue.Identity.UserDatabase/Deployment .
ENTRYPOINT ["/bin/bash", "entrypoint.sh"]
