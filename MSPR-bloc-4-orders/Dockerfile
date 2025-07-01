# IMAGE DE BASE POUR L'EXÉCUTION
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# IMAGE POUR LA CONSTRUCTION
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MSPR-bloc-4-orders/MSPR-bloc-4-orders.csproj", "MSPR-bloc-4-orders/"]
RUN dotnet restore "MSPR-bloc-4-orders/MSPR-bloc-4-orders.csproj"
COPY . .
WORKDIR "/src/MSPR-bloc-4-orders"
RUN dotnet build "MSPR-bloc-4-orders.csproj" -c %BUILD_CONFIGURATION% -o /app/build

# ÉTAPE DE PUBLICATION
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MSPR-bloc-4-orders.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# IMAGE FINALE
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSPR-bloc-4-orders.dll"]
