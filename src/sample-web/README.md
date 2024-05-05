
VITE_API_URL=http://localhost:5108 pnpm run dev

## Comando para fazer o build
docker build --build-arg API_URL=http://localhost:5108 -t gusflopes/sample-react . --no-cache


docker run -d --name sample-web -e API_PORT=5108 -p 6000:80 -p 6001:443  gusflopes/sample-react





### Build & Deploy

A aplicação está configurada para redirecionar as chamadas feitas na url principal onde acessa o frontend com um sufixo "/api" (ex: http://localhost:3000/api)

```bash
# Ao fazer o build estou informando a API_URL, que será na mesma porta da aplicação Web
docker build --build-arg API_URL=http://localhost:3000 -t gusflopes/sample-react . --no-cache

# Ao executar o container, informar qual será a porta da API que estará rodando
# no endereço http://host.docker.internal:${API_PORT}, ou seja, na máquina local
docker run -d --name sample-web -e API_PORT=5108 -p 3000:80 -p 6001:443  gusflopes/sample-react

```
