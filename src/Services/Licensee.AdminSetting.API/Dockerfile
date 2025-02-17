FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src

COPY ./src/Services/Licensee.AdminSetting.API/Licensee.AdminSetting.API.csproj ./Services/Licensee.AdminSetting.API/Licensee.AdminSetting.API.csproj

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore Services/Licensee.AdminSetting.API/Licensee.AdminSetting.API.csproj -v n

COPY ./src/Services/Licensee.AdminSetting.API/ ./Services/Licensee.AdminSetting.API/

ARG TRANSFORM_CONFIG
RUN echo $TRANSFORM_CONFIG
COPY "./src/Services/Licensee.AdminSetting.API/appsettings.$TRANSFORM_CONFIG.json" ./Services/Licensee.AdminSetting.API/appsettings.Production.json

WORKDIR /src/Services/Licensee.AdminSetting.API/

RUN dotnet publish --use-current-runtime --self-contained false -o /app  

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine-25h1 AS base

# Don’t run as root
RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
#

ENV COMPlus_EnableDiagnostics=0
WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Licensee.AdminSetting.API.dll"]
