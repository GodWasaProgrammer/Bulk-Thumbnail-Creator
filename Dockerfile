FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY BulkThumbnailCreator.sln .
COPY BulkThumbnailCreator.csproj .
COPY WebUI/WebUI.csproj ./WebUI/
COPY WebUI/wwwroot/ ./WebUI/wwwroot/

RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "WebUI.dll"]
