FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src

COPY ./src/Services/Fanex.Customer.API/Fanex.Customer.API.csproj ./Services/Fanex.Customer.API/Fanex.Customer.API.csproj
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ./src/Services/Fanex.Customer.API ./Services/Fanex.Customer.API

WORKDIR /src/Services/Fanex.Customer.API
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -o /app  

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine-25h1 AS base

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

ENTRYPOINT ["dotnet", "Fanex.Customer.API.dll"]
