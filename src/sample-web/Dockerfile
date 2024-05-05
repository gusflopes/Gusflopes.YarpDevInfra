# Estágio de compilação
FROM node:18 AS build

WORKDIR /app
RUN npm install -g pnpm

# Copiar arquivos de configuração
COPY package.json .
COPY pnpm-lock.yaml .
# COPY npm.lock .

# Instalar dependências
RUN pnpm install
# RUN npm install

# Copiar código-fonte
COPY . .

# Compilar aplicação
ENV VITE_API_URL=$API_URL
RUN pnpm build
# RUN npm build

# Estágio de produção
FROM nginx:alpine

USER root

# Copiar arquivos de compilação do estágio anterior
COPY --from=build /app/dist /usr/share/nginx/html
RUN chmod -R 755 /usr/share/nginx/html
RUN chown -R nginx:nginx /usr/share/nginx/html

# Configurar o Nginx
# COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/templates/default.conf.template

# Expor a porta 80
EXPOSE 80 443

# Comando de inicialização do Nginx
# CMD ["nginx", "-g", "daemon off;"]
CMD ["sh", "-c", "envsubst '${API_PORT}' < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
