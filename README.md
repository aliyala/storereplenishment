# StoreReplenishment

## Run on Windows
- dotnet publish StoreReplenishmentApi/StoreReplenishmentApi.csproj

- cd StoreReplenishmentApi\bin\Debug\net5.0\publish

- dotnet StoreReplenishmentApi.dll

Swagger will be available http://localhost:5000/swagger/index.html

## Run in docker
- docker build -t storereplenishment .

- docker run -p80:80 storereplenishment:latest

Swagger will be available http://localhost/swagger/index.html

## Test
- dotnet test --collect:"XPlat Code Coverage"
