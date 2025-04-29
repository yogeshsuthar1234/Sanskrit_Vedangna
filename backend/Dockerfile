# Use the official .NET 9.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the solution and project files
COPY . ./

# Restore dependencies from the API directory
RUN dotnet restore API/API.csproj

# Build and publish the app
RUN dotnet publish API/API.csproj -c Release -o out

# Use the official .NET 9.0 runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port that Render will assign (Render sets PORT env variable)
EXPOSE 5002

# Run the app
ENTRYPOINT ["dotnet", "API.dll"]