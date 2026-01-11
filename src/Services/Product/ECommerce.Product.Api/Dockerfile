FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/Product/ECommerce.Product.Api/ECommerce.Product.Api.csproj", "Services/Product/ECommerce.Product.Api/"]
COPY ["src/Services/Product/ECommerce.Product.Application/ECommerce.Product.Application.csproj", "Services/Product/ECommerce.Product.Application/"]
COPY ["src/Services/Product/ECommerce.Product.Domain/ECommerce.Product.Domain.csproj", "Services/Product/ECommerce.Product.Domain/"]
COPY ["src/Services/Product/ECommerce.Product.Infrastructure/ECommerce.Product.Infrastructure.csproj", "Services/Product/ECommerce.Product.Infrastructure/"]

RUN dotnet restore "Services/Product/ECommerce.Product.Api/ECommerce.Product.Api.csproj"

COPY src/ .

WORKDIR "/src/Services/Product/ECommerce.Product.Api"
RUN dotnet build "ECommerce.Product.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ECommerce.Product.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ECommerce.Product.Api.dll"]
