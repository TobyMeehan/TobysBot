FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mwader/static-ffmpeg:5.0-1 AS ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TobysBot.Discord/TobysBot.Discord.csproj", "TobysBot.Discord/"]
COPY ["TobysBot.Discord.Client/TobysBot.Discord.Client.csproj", "TobysBot.Discord.Client/"]
COPY ["TobysBot.Discord.Audio/TobysBot.Discord.Audio.csproj", "TobysBot.Discord.Audio/"]
RUN dotnet restore "TobysBot.Discord/TobysBot.Discord.csproj"
COPY . .
WORKDIR "/src/TobysBot.Discord"
RUN dotnet build "TobysBot.Discord.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TobysBot.Discord.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=mwader/static-ffmpeg:5.0.1 /ffmpeg /usr/local/bin/
COPY --from=mwader/static-ffmpeg:5.0.1 /ffprobe /usr/local/bin/
ENTRYPOINT ["dotnet", "TobysBot.Discord.dll"]
