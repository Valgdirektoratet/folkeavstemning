FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.targets", ""]
COPY ["Directory.Build.props", ""]
COPY ["Directory.Packages.props", ""]
COPY ["src/Stemmemottak/Stemmemottak.Frontend/Stemmemottak.Frontend.csproj", "src/Stemmemottak/Stemmemottak.Frontend/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/Stemmemottak/Stemmemottak.Frontend/Stemmemottak.Frontend.csproj"
COPY . .
WORKDIR "/src/src/Stemmemottak/Stemmemottak.Frontend"
RUN dotnet build "Stemmemottak.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stemmemottak.Frontend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /etc/ssl/certs
COPY --from=build /src/docker/cert .
WORKDIR /app
ENTRYPOINT ["dotnet", "Stemmemottak.Frontend.dll"]
