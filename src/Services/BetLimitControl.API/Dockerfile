FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src

COPY ./src/Services/BetLimitControl.API/BetLimitControl.API.csproj ./Services/BetLimitControl.API/BetLimitControl.API.csproj

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore Services/BetLimitControl.API/BetLimitControl.API.csproj -v n

COPY ./src/Services/BetLimitControl.API/ ./Services/BetLimitControl.API/

ARG TRANSFORM_CONFIG
RUN echo $TRANSFORM_CONFIG

WORKDIR /src/Services/BetLimitControl.API/

RUN dotnet publish --use-current-runtime --self-contained false -o /app  

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-slim-25h1 AS base

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

ENTRYPOINT ["dotnet", "BetLimitControl.API.dll"]
