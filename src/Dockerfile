FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-dotnet
WORKDIR /src
COPY ./YarpDevInfra.SampleAPI .

RUN dotnet restore
RUN dotnet build --configuration Release -o /app
RUN dotnet publish ./YarpDevInfra.SampleAPI.csproj --configuration Release -o /app

FROM node:18 AS build-node
WORKDIR /src
RUN npm install -g pnpm

COPY ./sample-web/package.json .
COPY ./sample-web/pnpm-lock.yaml .

RUN pnpm install

COPY ./sample-web .

ENV VITE_API_URL=$API_URL
RUN pnpm build

FROM nginx


## RunTime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-dotnet /app .
COPY ./YarpDevInfra.SampleAPI/appsettings.json .

ENTRYPOINT ["dotnet", "YarpDevInfra.SampleAPI.dll"]

#FROM node:18 AS build-node
