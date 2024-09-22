FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PokedexApi/PokedexApi.csproj", "PokedexApi/"]
RUN dotnet restore "PokedexApi/PokedexApi.csproj"
COPY . .
WORKDIR "/src/PokedexApi"
RUN dotnet build "PokedexApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/PokedexApi"
RUN dotnet publish "PokedexApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PokedexApi.dll"]