FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/BuildingBlocks/ECommerce.Contracts/ECommerce.Contracts.csproj src/BuildingBlocks/ECommerce.Contracts/
COPY src/BuildingBlocks/ECommerce.Messaging/ECommerce.Messaging.csproj src/BuildingBlocks/ECommerce.Messaging/
COPY src/BuildingBlocks/ECommerce.Observability/ECommerce.Observability.csproj src/BuildingBlocks/ECommerce.Observability/
COPY src/Services/Stock/ECommerce.Stock.Domain/ECommerce.Stock.Domain.csproj src/Services/Stock/ECommerce.Stock.Domain/
COPY src/Services/Stock/ECommerce.Stock.Application/ECommerce.Stock.Application.csproj src/Services/Stock/ECommerce.Stock.Application/
COPY src/Services/Stock/ECommerce.Stock.Infrastructure/ECommerce.Stock.Infrastructure.csproj src/Services/Stock/ECommerce.Stock.Infrastructure/
COPY src/Services/Stock/ECommerce.Stock.Worker/ECommerce.Stock.Worker.csproj src/Services/Stock/ECommerce.Stock.Worker/
RUN dotnet restore src/Services/Stock/ECommerce.Stock.Worker/ECommerce.Stock.Worker.csproj
COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Stock/ src/Services/Stock/
RUN dotnet publish src/Services/Stock/ECommerce.Stock.Worker/ECommerce.Stock.Worker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.Stock.Worker.dll"]
