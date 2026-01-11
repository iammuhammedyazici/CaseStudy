FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/BuildingBlocks/ECommerce.Observability/ECommerce.Observability.csproj src/BuildingBlocks/ECommerce.Observability/
COPY src/ApiGateway/ECommerce.ApiGateway/ECommerce.ApiGateway.csproj src/ApiGateway/ECommerce.ApiGateway/
RUN dotnet restore src/ApiGateway/ECommerce.ApiGateway/ECommerce.ApiGateway.csproj
COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/ApiGateway/ src/ApiGateway/
RUN dotnet publish src/ApiGateway/ECommerce.ApiGateway/ECommerce.ApiGateway.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.ApiGateway.dll"]
