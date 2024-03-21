#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 7088
ENV ASPNETCORE_URLS http://+:7088
ENV ASPNETCORE_ENVIRONMENT Release
ENV CACHE_SIZE_LIMIT 1073741824
ENV RefitRoute__FirstFlightSevice https://firstflight.azurewebsites.net
ENV RefitRoute__SecondFlightSevice https://secondflightservice.azurewebsites.net

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY AirTravelAggregatorAPI.csproj AirTravelAggregatorAPI.csproj
RUN dotnet restore AirTravelAggregatorAPI.csproj
COPY . .
WORKDIR /src
RUN dotnet build AirTravelAggregatorAPI.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish AirTravelAggregatorAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AirTravelAggregatorAPI.dll"]