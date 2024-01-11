ARG BUILDPLATFORM=linux/amd64

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine@sha256:82a31b3b04d4ee1a50496ed620652cef597b636f27c7274d2bdf6d1b9962cf4a AS build
WORKDIR /src
COPY ingress-nginx-validate-jwt.csproj .
RUN dotnet restore "ingress-nginx-validate-jwt.csproj"
COPY . .
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine@sha256:ddf059bfc910d52288102cc50315e97e6cd065b1e2fe2056e0ecf8f4c678d7ee AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

RUN apk update && \
    apk upgrade && \
    rm -rf /var/cache/apk/*

USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["./ingress-nginx-validate-jwt"]
