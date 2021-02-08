# storereplenishment
storereplenishment

## Run
- dotnet publish StoreReplenishmentApi/StoreReplenishmentApi.csproj

- cd StoreReplenishmentApi\bin\Debug\netcoreapp3.1\publish

- dotnet StoreReplenishmentApi.dll

## Run in docker
- docker build -t storereplenishment .

- docker run -p80:80 storereplenishment:latest

## Test
- dotnet test --collect:"XPlat Code Coverage"
