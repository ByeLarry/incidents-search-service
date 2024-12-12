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
COPY ["SearchService/SearchService.csproj", "SearchService/"]
RUN dotnet restore "./SearchService/SearchService.csproj"
COPY . .
WORKDIR "/src/SearchService"
RUN dotnet build "./SearchService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикация приложения
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SearchService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

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
ENTRYPOINT ["dotnet", "SearchService.dll"]
