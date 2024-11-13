# Определение аргументов
ARG LAUNCHING_FROM_VS
ARG FINAL_BASE_IMAGE=${LAUNCHING_FROM_VS:+aotdebug}

# Базовый образ для отладки
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["api/api.csproj", "api/"]
RUN dotnet restore "./api/api.csproj"
COPY . .
WORKDIR "/src/api"
RUN dotnet build "./api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикация приложения
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

# Конфигурация для отладки
FROM base as aotdebug
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    gdb
USER app

# Финальный этап с использованием ASP.NET Runtime
FROM ${FINAL_BASE_IMAGE:-mcr.microsoft.com/dotnet/aspnet:8.0} AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.dll"]
