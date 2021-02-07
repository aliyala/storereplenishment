FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
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