FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.targets", ""]
COPY ["Directory.Build.props", ""]
COPY ["Directory.Packages.props", ""]
COPY ["src/Folkeavstemning/Folkeavstemning.Backend/Folkeavstemning.Backend.csproj", "src/Folkeavstemning/Folkeavstemning.Backend/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/Manntall/Manntall.Client/Manntall.Client.csproj", "src/Manntall/Manntall.Client/"]
RUN dotnet restore "src/Folkeavstemning/Folkeavstemning.Backend/Folkeavstemning.Backend.csproj"
COPY . .
WORKDIR "/src/src/Folkeavstemning/Folkeavstemning.Backend"
RUN dotnet build "Folkeavstemning.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Folkeavstemning.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /etc/ssl/certs
COPY --from=build /src/docker/cert .
WORKDIR /app
ENTRYPOINT ["dotnet", "Folkeavstemning.Backend.dll"]
