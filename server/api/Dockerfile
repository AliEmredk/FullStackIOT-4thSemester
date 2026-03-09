FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY server/api/api.csproj server/api/
COPY server/dataaccess/dataaccess.csproj server/dataaccess/

RUN dotnet restore server/api/api.csproj

COPY server/api/ server/api/
COPY server/dataaccess/ server/dataaccess/

RUN dotnet publish server/api/api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "api.dll"]