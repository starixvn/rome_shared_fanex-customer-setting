# syntax=docker/dockerfile:1.4

# 1. Build apps
FROM hub.nexdev.net/base/node:18.16-alpine AS builder
WORKDIR /app

COPY src/Web/customer-web-app/package.json ./
COPY src/Web/customer-web-app/package-lock.json ./

RUN npm install

# Register TRANSFORM_CONFIG argument for transforming by environment
ARG TRANSFORM_CONFIG=production
ARG DEPLOY_ENV=PROD

RUN echo $TRANSFORM_CONFIG
RUN echo $DEPLOY_ENV

COPY src/Web/customer-web-app .
COPY src/Web/customer-web-app/.env.$DEPLOY_ENV ./apps/create-super/.env.production
COPY src/Web/customer-web-app/.env.$DEPLOY_ENV ./apps/create-customer/.env.production
COPY src/Web/customer-web-app/.env.$DEPLOY_ENV ./apps/edit-customer/.env.production
COPY src/Web/customer-web-app/.env.$DEPLOY_ENV ./apps/edit-multiple-customers/.env.production
COPY src/Web/customer-web-app/.env.$DEPLOY_ENV ./apps/sportsbook-information/.env.production

RUN npx nx run-many -t build --mode $TRANSFORM_CONFIG

# 2. Nginx
FROM hub.nexdev.net/base/nginxinc/nginx-unprivileged:1.22-alpine-slim

# Set root user has full permissions of read/write files/folders to copy needed things
USER root

# Copy config nginx
COPY --from=builder /app/.nginx/nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

# Remove default nginx static assets
RUN rm -rf ./*

# Copy static assets from builder stage
COPY --from=builder /app/dist/apps .

WORKDIR /usr/share/nginx/html

# Containers run nginx with global directives and daemon off
ENTRYPOINT ["nginx", "-g", "daemon off;"]