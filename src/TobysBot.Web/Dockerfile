FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["./src/TobysBot.Web/TobysBot.Web.csproj", "./TobysBot.Web/"]
RUN dotnet restore "TobysBot.Web/TobysBot.Web.csproj"
COPY . .
WORKDIR "/src/src/TobysBot.Web"
RUN dotnet build "TobysBot.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TobysBot.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TobysBot.Web.dll"]
