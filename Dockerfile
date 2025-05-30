FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

COPY . /source
WORKDIR /source

ARG TARGETARCH

RUN dotnet restore ShitChat.sln --verbosity minimal && \
    cd ShitChat.Api && \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app


FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final

WORKDIR /app

RUN apk add --no-cache icu-libs

COPY --from=build /app .

ENTRYPOINT ["dotnet", "ShitChat.Api.dll"]
