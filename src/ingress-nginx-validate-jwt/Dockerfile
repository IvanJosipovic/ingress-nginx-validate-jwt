FROM mcr.microsoft.com/dotnet/sdk:6.0.407-alpine3.17@sha256:67b87a054017d5c5de49a127d8206470a40adb0c9fbc099a130c91f76588484c AS build
WORKDIR /src
COPY ["ingress-nginx-validate-jwt/ingress-nginx-validate-jwt.csproj", "ingress-nginx-validate-jwt/"]
RUN dotnet restore "ingress-nginx-validate-jwt/ingress-nginx-validate-jwt.csproj"
COPY . .
WORKDIR "/src/ingress-nginx-validate-jwt"
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime --self-contained true -o /app/publish /p:Version=${VERSION} /p:PublishTrimmed=false

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.15-alpine3.17@sha256:fa822a9f909e163917767261111bc02159334fd0263304f561f2e1689af3cdf3 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
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
