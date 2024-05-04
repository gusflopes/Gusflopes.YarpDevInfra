## Criando projetos


```bash
dotnet new webapi --name YarpDevInfra.GatewayAPI --output ./src/YarpDevInfra.GatewayAPI
dotnet new webapi --name YarpDevInfra.SampleAPI --output ./src/backend/YarpDevInfra.SampleAPI
```
docker run -d --name sample-api2 -p 5001:8080  gusflopes/yarp-sample

dotnet sln add ./src/YarpDevInfra.GatewayAPI/YarpDevInfra.GatewayAPI.csproj
