FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /source

COPY *.sln .
COPY ShitChat.Api/*.csproj ShitChat.Api/
COPY ShitChat.Application/*.csproj ShitChat.Application/
COPY ShitChat.Domain/*.csproj ShitChat.Domain/
COPY ShitChat.Infrastructure/*.csproj ShitChat.Infrastructure/
COPY ShitChat.Shared/*.csproj ShitChat.Shared/
RUN dotnet restore ShitChat.sln --verbosity minimal

COPY . .
ARG TARGETARCH
RUN dotnet publish ShitChat.Api -c Release -a ${TARGETARCH/amd64/x64} \
    --use-current-runtime --self-contained false -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

RUN apk add --no-cache icu-libs

COPY --from=build /app .
ENTRYPOINT ["dotnet", "ShitChat.Api.dll"]