FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src

COPY ./src/ApiGateways/Deposit.ApiGateways/Deposit.ApiGateways.csproj ./ApiGateways/Deposit.ApiGateways/Deposit.ApiGateways.csproj

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore ApiGateways/Deposit.ApiGateways -v n

COPY ./src/ApiGateways/Deposit.ApiGateways/ ./ApiGateways/Deposit.ApiGateways/

ARG TRANSFORM_CONFIG
RUN echo $TRANSFORM_CONFIG
COPY "./src/ApiGateways/Deposit.ApiGateways/appsettings.$TRANSFORM_CONFIG.json" ./ApiGateways/Deposit.ApiGateways/appsettings.Production.json
COPY "./src/ApiGateways/Deposit.ApiGateways/ocelot.$TRANSFORM_CONFIG.json" ./ApiGateways/Deposit.ApiGateways/ocelot.Production.json

WORKDIR /src/ApiGateways/Deposit.ApiGateways/


RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish --use-current-runtime --self-contained false -o /app  

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS base

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

ENTRYPOINT ["dotnet", "Deposit.ApiGateways.dll"]
