#!/bin/sh
mkcert -install

ip route|awk '/default/ { print $3 " host.docker.internal"}' >> /etc/hosts

dotnet NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.dll
