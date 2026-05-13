FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./IncidentService.csproj ./IncidentService.csproj
RUN dotnet restore IncidentService.csproj

COPY . ./

RUN dotnet publish IncidentService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "IncidentService.dll"]