# Stage 1: Build your .NET 9 app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime + Syft installed
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install Syft
RUN apt-get update && apt-get install -y curl \
    && curl -sSfL https://raw.githubusercontent.com/anchore/syft/main/install.sh \
    | sh -s -- -b /usr/local/bin \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# Optional: Verify Syft installation during build
RUN syft --version

# Copy published app from build stage
COPY --from=build /app/publish .

# Run your app
ENTRYPOINT ["dotnet", "YourApp.dll"]
