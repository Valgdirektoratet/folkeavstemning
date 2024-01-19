FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.targets", ""]
COPY ["Directory.Build.props", ""]
COPY ["Directory.Packages.props", ""]
COPY ["src/Stemmemottak/Stemmemottak.Backend/Stemmemottak.Backend.csproj", "src/Stemmemottak/Stemmemottak.Backend/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/Stemmemottak/Stemmemottak.Backend/Stemmemottak.Backend.csproj"
COPY . .
WORKDIR "/src/src/Stemmemottak/Stemmemottak.Backend"
RUN dotnet build "Stemmemottak.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stemmemottak.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /etc/ssl/certs
COPY --from=build /src/docker/cert .
WORKDIR /app
ENTRYPOINT ["dotnet", "Stemmemottak.Backend.dll"]
