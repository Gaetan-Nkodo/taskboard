# -----------------------------
# 1) BASE RUNTIME (léger)
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# -----------------------------
# 2) BUILD STAGE (SDK)
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier uniquement les csproj pour optimiser le cache Docker
COPY Sources/TaskBoard.Api/TaskBoard.Api.csproj Sources/TaskBoard.Api/
COPY Sources/TaskBoard.Domain/TaskBoard.Domain.csproj Sources/TaskBoard.Domain/
COPY Sources/TaskBoard.Application/TaskBoard.Application.csproj Sources/TaskBoard.Application/
COPY Sources/TaskBoard.Infrastructure/TaskBoard.Infrastructure.csproj Sources/TaskBoard.Infrastructure/

# Restore (cache efficace)
RUN dotnet restore Sources/TaskBoard.Api/TaskBoard.Api.csproj

# Copier le reste du code
COPY . .

# Build
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

# Copier les fichiers publiés
COPY --from=publish /app/publish .

# Sécurité : utilisateur non-root (si tu veux)
# USER $APP_UID

ENTRYPOINT ["dotnet", "TaskBoard.Api.dll"]
