# syntax=docker/dockerfile:1.4

# 1. Build apps
FROM hub.nexdev.net/base/node:18.16-alpine AS builder
WORKDIR /app

COPY src/Web/customer-web-app/package.json ./
COPY src/Web/customer-web-app/package-lock.json ./
RUN npm install

# Register TRANSFORM_CONFIG argument for transforming by environment
ARG TRANSFORM_CONFIG=production

RUN echo $TRANSFORM_CONFIG

COPY src/Web/customer-web-app .
COPY src/Web/customer-web-app/.env.$TRANSFORM_CONFIG ./apps/create-customer/.env.production

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