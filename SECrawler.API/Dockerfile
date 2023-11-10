FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SECrawler.API/SECrawler.API.csproj", "SECrawler.API/"]
RUN dotnet restore "SECrawler.API/SECrawler.API.csproj"
COPY . .
WORKDIR "/src/SECrawler.API"
RUN dotnet build "SECrawler.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SECrawler.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SECrawler.API.dll"]
