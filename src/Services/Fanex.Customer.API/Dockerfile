# https://hub.docker.com/_/microsoft-dotnet
FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/Services/Fanex.Customer.API/*.csproj ./Fanex.Customer.API/
RUN dotnet restore Fanex.Customer.API/Fanex.Customer.API.csproj

# copy and build app and libraries
COPY src/Services/Fanex.Customer.API ./Fanex.Customer.API/
WORKDIR /source/Fanex.Customer.API
RUN dotnet publish -c release -o /app -r linux-x64 --self-contained false --no-restore

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
#

# final stage/image
FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Customer.API.dll"]