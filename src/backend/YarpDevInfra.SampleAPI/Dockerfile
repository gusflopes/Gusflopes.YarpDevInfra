FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# COPY ./*.sln .
COPY . .

RUN dotnet restore

RUN dotnet build --configuration Release -o /app

RUN dotnet publish ./YarpDevInfra.SampleAPI.csproj --configuration Release -o /app

# RunTime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app .
COPY ./appsettings.json .

# Exposes 80,443 por padrão ao rodar em container
# Ou seria 8080 ?

ENTRYPOINT ["dotnet", "YarpDevInfra.SampleAPI.dll"]

