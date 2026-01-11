FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/BuildingBlocks/ECommerce.Contracts/ECommerce.Contracts.csproj src/BuildingBlocks/ECommerce.Contracts/
COPY src/BuildingBlocks/ECommerce.Messaging/ECommerce.Messaging.csproj src/BuildingBlocks/ECommerce.Messaging/
COPY src/BuildingBlocks/ECommerce.Observability/ECommerce.Observability.csproj src/BuildingBlocks/ECommerce.Observability/
COPY src/Services/Notification/ECommerce.Notification.Domain/ECommerce.Notification.Domain.csproj src/Services/Notification/ECommerce.Notification.Domain/
COPY src/Services/Notification/ECommerce.Notification.Application/ECommerce.Notification.Application.csproj src/Services/Notification/ECommerce.Notification.Application/
COPY src/Services/Notification/ECommerce.Notification.Infrastructure/ECommerce.Notification.Infrastructure.csproj src/Services/Notification/ECommerce.Notification.Infrastructure/
COPY src/Services/Notification/ECommerce.Notification.Worker/ECommerce.Notification.Worker.csproj src/Services/Notification/ECommerce.Notification.Worker/
RUN dotnet restore src/Services/Notification/ECommerce.Notification.Worker/ECommerce.Notification.Worker.csproj
COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Notification/ src/Services/Notification/
RUN dotnet publish src/Services/Notification/ECommerce.Notification.Worker/ECommerce.Notification.Worker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.Notification.Worker.dll"]
