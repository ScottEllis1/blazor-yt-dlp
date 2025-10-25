# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Switch to root for package install
USER root

# Install external dependencies
RUN apt-get update && \
    apt-get install -y python3 python3-pip ffmpeg && \
    pip install --break-system-packages -U yt-dlp && \
    rm -rf /var/lib/apt/lists/*

USER app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlazorLab/BlazorLab.csproj", "BlazorLab/"]
RUN dotnet restore "./BlazorLab/BlazorLab.csproj"
COPY . .
WORKDIR "/src/BlazorLab"
RUN dotnet build "./BlazorLab.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlazorLab.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorLab.dll"]