# https://hub.docker.com/_/microsoft-dotnet
FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/Services/Fanex.Auth.API/*.csproj ./Fanex.Auth.API/
RUN dotnet restore Fanex.Auth.API/Fanex.Auth.API.csproj

# copy and build app and libraries
COPY src/Services/Fanex.Auth.API ./Fanex.Auth.API/
WORKDIR /source/Fanex.Auth.API
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
COPY --from=builder /app ./
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Auth.API.dll"]