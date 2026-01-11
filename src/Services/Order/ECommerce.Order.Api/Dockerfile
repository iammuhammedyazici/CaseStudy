FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/BuildingBlocks/ECommerce.Contracts/ECommerce.Contracts.csproj src/BuildingBlocks/ECommerce.Contracts/
COPY src/BuildingBlocks/ECommerce.Messaging/ECommerce.Messaging.csproj src/BuildingBlocks/ECommerce.Messaging/
COPY src/BuildingBlocks/ECommerce.Observability/ECommerce.Observability.csproj src/BuildingBlocks/ECommerce.Observability/
COPY src/Services/Order/ECommerce.Order.Domain/ECommerce.Order.Domain.csproj src/Services/Order/ECommerce.Order.Domain/
COPY src/Services/Order/ECommerce.Order.Application/ECommerce.Order.Application.csproj src/Services/Order/ECommerce.Order.Application/
COPY src/Services/Order/ECommerce.Order.Infrastructure/ECommerce.Order.Infrastructure.csproj src/Services/Order/ECommerce.Order.Infrastructure/
COPY src/Services/Order/ECommerce.Order.Api/ECommerce.Order.Api.csproj src/Services/Order/ECommerce.Order.Api/
RUN dotnet restore src/Services/Order/ECommerce.Order.Api/ECommerce.Order.Api.csproj
COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Order/ src/Services/Order/
RUN dotnet publish src/Services/Order/ECommerce.Order.Api/ECommerce.Order.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.Order.Api.dll"]
