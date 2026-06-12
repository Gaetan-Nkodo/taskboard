# -----------------------------
# 1) BASE RUNTIME (Alpine .NET 8)
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

# Sécurité : mise à jour des libs Alpine (corrige libcrypto3/libssl3)
RUN apk update && apk upgrade

WORKDIR /app
EXPOSE 8080

# -----------------------------
# 2) BUILD STAGE (SDK .NET 8 Alpine)
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Sources/TaskBoard.Api/TaskBoard.Api.csproj Sources/TaskBoard.Api/
COPY Sources/TaskBoard.Domain/TaskBoard.Domain.csproj Sources/TaskBoard.Domain/
COPY Sources/TaskBoard.Application/TaskBoard.Application.csproj Sources/TaskBoard.Application/
COPY Sources/TaskBoard.Infrastructure/TaskBoard.Infrastructure.csproj Sources/TaskBoard.Infrastructure/

RUN dotnet restore Sources/TaskBoard.Api/TaskBoard.Api.csproj

COPY . .

WORKDIR /src/Sources/TaskBoard.Api
RUN dotnet build TaskBoard.Api.csproj -c $BUILD_CONFIGURATION -o /app/build

# -----------------------------
# 3) PUBLISH STAGE
# -----------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish TaskBoard.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -----------------------------
# 4) FINAL RUNTIME
# -----------------------------
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TaskBoard.Api.dll"]
