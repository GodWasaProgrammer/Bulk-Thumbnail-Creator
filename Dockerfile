# Stage 1: Build .NET application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebUI/WebUI.csproj", "WebUI/"]
COPY ["BulkThumbnailCreator.csproj", "."]
RUN dotnet restore "./WebUI/./WebUI.csproj"
COPY . .
WORKDIR "/src/WebUI"
RUN dotnet build "./WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish .NET application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Install Python and set up the final image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
ARG BUILD_CONFIGURATION=Release

# Install Dependencies
RUN apt-get update \
    && apt-get install -y \
        python3 \
        libx11-6 \
        libopenblas-dev \
        libgdiplus/* \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "WebUI.dll"]