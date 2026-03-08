# # Build stage
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /app

# # Copy solution and project files
# COPY backend/src/*.sln ./  
# COPY backend/src/MoneyPilot.Infrastructure/*.csproj ./MoneyPilot.Infrastructure/
# COPY backend/src/MoneyPilot.API/*.csproj ./MoneyPilot.API/
# # COPY backend/src/moneypilot.API/*.csproj ./moneypilot.API/

# # Restore dependencies
# RUN dotnet restore

# # Copy everything else and publish
# COPY backend/src/ ./
# RUN dotnet publish MoneyPilot.API -c Release -o out
# # RUN dotnet publish moneypilot.API -c Release -o out

# # Runtime stage
# FROM mcr.microsoft.com/dotnet/aspnet:8.0
# WORKDIR /app
# COPY --from=build /app/out .
# EXPOSE 8080
# ENV ASPNETCORE_URLS=http://+:8080
# ENTRYPOINT ["dotnet", "MoneyPilot.API.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution file
COPY backend/src/*.sln ./

# Copy all project files (preserve folder structure)
COPY backend/src/MoneyPilot.API/*.csproj ./MoneyPilot.API/
COPY backend/src/MoneyPilot.Application/*.csproj ./MoneyPilot.Application/
COPY backend/src/MoneyPilot.Infrastructure/*.csproj ./MoneyPilot.Infrastructure/
COPY backend/src/MoneyPilot.Domain/*.csproj ./MoneyPilot.Domain/
COPY backend/src/MoneyPilot.Tests/*.csproj ./MoneyPilot.Tests/
COPY backend/src/MoneyPilot.SecurityHeaders/*.csproj ./MoneyPilot.SecurityHeaders/

# Restore dependencies
RUN dotnet restore

# Copy everything else and publish
COPY backend/src/ ./
RUN dotnet publish MoneyPilot.API -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "MoneyPilot.API.dll"]