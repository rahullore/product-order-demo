# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only csproj first (better layer caching)
COPY CustomerOrders.Api/*.csproj ./CustomerOrders.Api/
RUN dotnet restore ./CustomerOrders.Api/CustomerOrders.Api.csproj

# Copy everything else
COPY . .
RUN dotnet publish ./CustomerOrders.Api/CustomerOrders.Api.csproj -c Release -o /app/publish --no-restore

# Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "CustomerOrders.Api.dll"]

