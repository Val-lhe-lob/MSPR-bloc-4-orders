# Étape 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie tous les fichiers
COPY . .

# Restore et publish
RUN dotnet restore MSPR-bloc-4-orders.csproj
RUN dotnet publish MSPR-bloc-4-orders.csproj -c Release -o /app/publish

# Étape 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "MSPR-bloc-4-orders.dll"]
