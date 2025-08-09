# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER root
RUN mkdir -p /app/shared && chmod -R 777 /app/shared
RUN apt-get update && apt-get install -y ffmpeg && apt-get clean && rm -rf /var/lib/apt/lists/*
USER $APP_UID

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["H1Assist/H1Assist.csproj", "H1Assist/"]
RUN dotnet restore "./H1Assist/H1Assist.csproj"
COPY . .
WORKDIR "/src/H1Assist"
#COPY H1Assist/package.json H1Assist/package-lock.json ./
#RUN apt-get update && apt-get install -y nodejs npm
#RUN npm install
#RUN if [ "$BUILD_CONFIGURATION" = "Release" ]; then npm run css:release; fi
RUN dotnet build "./H1Assist.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./H1Assist.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "H1Assist.dll"]