#!/bin/sh
mkcert -install

ip route|awk '/default/ { print $3 " host.docker.internal"}' >> /etc/hosts

dotnet /app/NHSD.BuyingCatalogue.Identity.Api.SampleResource.dll
