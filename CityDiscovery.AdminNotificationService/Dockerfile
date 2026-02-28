

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["CityDiscovery.AdminNotificationService/CityDiscovery.AdminNotificationService.csproj", "CityDiscovery.AdminNotificationService/"]
RUN dotnet restore "CityDiscovery.AdminNotificationService/CityDiscovery.AdminNotificationService.csproj"

COPY . .


WORKDIR "/src/CityDiscovery.AdminNotificationService"
RUN dotnet build "CityDiscovery.AdminNotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CityDiscovery.AdminNotificationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CityDiscovery.AdminNotificationService.dll"]