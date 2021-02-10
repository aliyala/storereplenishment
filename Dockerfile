FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["StoreReplenishmentApi/StoreReplenishmentApi.csproj", "StoreReplenishmentApi/"]
RUN dotnet restore "StoreReplenishmentApi/StoreReplenishmentApi.csproj"
COPY . .
WORKDIR "/src/StoreReplenishmentApi"
RUN dotnet build "StoreReplenishmentApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StoreReplenishmentApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreReplenishmentApi.dll"]