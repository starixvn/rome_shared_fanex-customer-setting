FROM hub.nexdev.net/base/dotnet/core/sdk:7.0-alpine AS builder

WORKDIR /src

COPY ./src/BuildingBlocks/ONEbook.NG2.API/ONEbook.NG2.API.csproj ./BuildingBlocks/ONEbook.NG2.API/ONEbook.NG2.API.csproj
COPY ./src/BuildingBlocks/ONEbook.NG2.API.Client/ONEbook.NG2.API.Client.csproj ./BuildingBlocks/ONEbook.NG2.API.Client/ONEbook.NG2.API.Client.csproj
COPY ./src/BuildingBlocks/ONEbook.UM.ThirdParty.AspNetCore/ONEbook.UM.ThirdParty.AspNetCore.csproj ./BuildingBlocks/ONEbook.UM.ThirdParty.AspNetCore/ONEbook.UM.ThirdParty.AspNetCore.csproj
COPY ./src/Services/Fanex.Customer.API/Fanex.Customer.API.csproj ./Services/Fanex.Customer.API/Fanex.Customer.API.csproj
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore Services/Fanex.Customer.API/Fanex.Customer.API.csproj -v n

COPY ./src/BuildingBlocks/ONEbook.NG2.API ./BuildingBlocks/ONEbook.NG2.API
COPY ./src/BuildingBlocks/ONEbook.NG2.API.Client ./BuildingBlocks/ONEbook.NG2.API.Client
COPY ./src/BuildingBlocks/ONEbook.UM.ThirdParty.AspNetCore ./BuildingBlocks/ONEbook.UM.ThirdParty.AspNetCore
COPY ./src/Services/Fanex.Customer.API ./Services/Fanex.Customer.API

WORKDIR /src/Services/Fanex.Customer.API

RUN dotnet publish -r linux-x64 --self-contained false -o /app 

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-alpine AS base

# Don’t run as root
RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Customer.API.dll"]
