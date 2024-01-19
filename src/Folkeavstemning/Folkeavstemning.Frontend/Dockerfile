FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM node:20-alpine AS buildnpm
WORKDIR /src
COPY ["src/frontend", ""]
WORKDIR /src/frontend
RUN npm install
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.targets", ""]
COPY ["Directory.Build.props", ""]
COPY ["Directory.Packages.props", ""]
COPY ["src/Folkeavstemning/Folkeavstemning.Frontend/Folkeavstemning.Frontend.csproj", "src/Folkeavstemning/Folkeavstemning.Frontend/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/Folkeavstemning/Folkeavstemning.Frontend/Folkeavstemning.Frontend.csproj"
COPY . .
WORKDIR "/src/src/Folkeavstemning/Folkeavstemning.Frontend"
RUN dotnet build "Folkeavstemning.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Folkeavstemning.Frontend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=buildnpm /src/dist ./wwwroot
WORKDIR /etc/ssl/certs
COPY --from=build /src/docker/cert .
WORKDIR /app
ENTRYPOINT ["dotnet", "Folkeavstemning.Frontend.dll"]
