## Criando projetos


```bash
dotnet new webapi --name YarpDevInfra.GatewayAPI --output ./src/YarpDevInfra.GatewayAPI
dotnet new webapi --name YarpDevInfra.SampleAPI --output ./src/backend/YarpDevInfra.SampleAPI
```
docker run -d --name sample-api2 -p 5001:8080  gusflopes/yarp-sample

dotnet sln add ./src/YarpDevInfra.GatewayAPI/YarpDevInfra.GatewayAPI.csproj


### Passo a Passo

Configurando o projeto para rodar sem o YARP

```bash
# A partir da raiz do repositório
cd ./src/YarpDevInfra.SampleAPI

# Atenção, a porta definida aqui precisa ser passada para o frontend
docker build -t gusflopes/sample-api . --no-cache
docker run -d --name demo-api -p 5100:8080 gusflopes/sample-api

# Acessar o repositório do frontend
cd ../sample-web

#  Utilizando a porta definida na etapa anterior
docker build --build-arg API_URL=http://localhost:5100/api -t gusflopes/sample-react . --no-cache
docker run -d --name sample-web -e API_PORT=5100 -p 4000:80 gusflopes/sample-react

# Caso esteja utilizando o YARP para várias versões, alguns ajustes são necessários

```
