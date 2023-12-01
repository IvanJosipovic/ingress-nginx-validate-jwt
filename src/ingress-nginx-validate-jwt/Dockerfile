ARG BUILDPLATFORM=linux/amd64

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine@sha256:1655c8f2784cf17f81191a9fd80fbf2e3f2ee1c2c86f174714635963986cb96b AS build
WORKDIR /src
COPY ingress-nginx-validate-jwt.csproj .
RUN dotnet restore "ingress-nginx-validate-jwt.csproj"
COPY . .
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine@sha256:f40ba3676c3dbbe013e03a765aab1c2dad7b72979412961dc81f086461759ce0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

RUN apk update && \
    apk upgrade && \
    rm -rf /var/cache/apk/*

USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["./ingress-nginx-validate-jwt"]
