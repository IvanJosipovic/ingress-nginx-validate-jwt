ARG BUILDPLATFORM=linux/amd64

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0-alpine@sha256:e3b051cbad561cec1b1ce3586aaf1279aeda72c2416f41c909d293cddf21c011 AS build
WORKDIR /src
COPY ingress-nginx-validate-jwt.csproj .
RUN dotnet restore "ingress-nginx-validate-jwt.csproj"
COPY . .
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine@sha256:e86ce2bf9b77a93075dbe03bc5a3ba4eeae93bdfc5415a9354c22ed3504d46a8 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

RUN apk update && \
    apk upgrade && \
    rm -rf /var/cache/apk/*

RUN addgroup -g 2000 appgroup \
    && adduser -u 1000 -G appgroup -D "appuser"

RUN chown appuser:appgroup  /app
USER appuser:appgroup

COPY --from=publish /app/publish .
ENTRYPOINT ["./ingress-nginx-validate-jwt"]
