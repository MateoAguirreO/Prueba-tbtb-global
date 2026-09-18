FROM node:20-alpine AS build
WORKDIR /src
COPY package*.json .
RUN npm ci
COPY . .
RUN npm run build -- --output-path=dist/web

FROM nginx:alpine AS final
COPY --from=build /src/dist/web/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
