# syntax=docker/dockerfile:1

# Build stage
FROM node:18-alpine AS build

WORKDIR /app

COPY ./frontend/package.json ./frontend/package-lock.json ./

RUN npm install

COPY ./frontend ./

RUN npm run build

# Final stage
FROM node:18-alpine AS final

WORKDIR /app

COPY --from=build /app ./

CMD ["npm", "start"]