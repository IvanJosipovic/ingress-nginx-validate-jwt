ARG BUILDPLATFORM=linux/amd64

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0-alpine@sha256:5ee4f48a942b22cd0887fe4de461df1ee7efb5c5ece530a441fa9626e191716c AS build
WORKDIR /src
COPY ingress-nginx-validate-jwt.csproj .
RUN dotnet restore "ingress-nginx-validate-jwt.csproj"
COPY . .
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine@sha256:3356f87295691a5b4f46de0c21ed8a00a3adbf3e46db21830944a8575d70d901 AS base
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
