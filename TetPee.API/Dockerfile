FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TetPee.API/TetPee.API.csproj", "TetPee.API/"]
COPY ["TetPee.Service/TetPee.Service.csproj", "TetPee.Service/"]
COPY ["TetPee.Repository/TetPee.Repository.csproj", "TetPee.Repository/"]
RUN dotnet restore "TetPee.API/TetPee.API.csproj"
COPY . .
WORKDIR "/src/TetPee.API"
RUN dotnet build "./TetPee.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TetPee.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TetPee.API.dll"]
