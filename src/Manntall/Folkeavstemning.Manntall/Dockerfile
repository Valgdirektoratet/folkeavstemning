FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.targets", ""]
COPY ["Directory.Build.props", ""]
COPY ["Directory.Packages.props", ""]
COPY ["src/Manntall/Folkeavstemning.Manntall/Folkeavstemning.Manntall.csproj", "src/Manntall/Folkeavstemning.Manntall/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/Manntall/Folkeavstemning.Manntall/Folkeavstemning.Manntall.csproj"
COPY . .
WORKDIR "/src/src/Manntall/Folkeavstemning.Manntall"
RUN dotnet build "Folkeavstemning.Manntall.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Folkeavstemning.Manntall.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /etc/ssl/certs
COPY --from=build /src/docker/cert .
WORKDIR /app
ENTRYPOINT ["dotnet", "Folkeavstemning.Manntall.dll"]
