FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Stock/ src/Services/Stock/

RUN dotnet restore src/Services/Stock/ECommerce.Stock.Api/ECommerce.Stock.Api.csproj
RUN dotnet publish src/Services/Stock/ECommerce.Stock.Api/ECommerce.Stock.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ECommerce.Stock.Api.dll"]
