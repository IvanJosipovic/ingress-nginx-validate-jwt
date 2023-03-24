FROM mcr.microsoft.com/dotnet/sdk:7.0.202-alpine3.17@sha256:d53bfcc2c90bc8d6991eea1ff5d9d904d42d927077978db4e7281d5269264b56 AS build
WORKDIR /src
COPY ["ingress-nginx-validate-jwt/ingress-nginx-validate-jwt.csproj", "ingress-nginx-validate-jwt/"]
RUN dotnet restore "ingress-nginx-validate-jwt/ingress-nginx-validate-jwt.csproj"
COPY . .
WORKDIR "/src/ingress-nginx-validate-jwt"
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime --self-contained true -o /app/publish /p:Version=${VERSION} /p:PublishTrimmed=false

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.15-alpine3.17@sha256:390e72a104a47e9137c6c5867dbe849ff9164bc49e3cb56c1bbb55aeca5d6a97 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
ENV ASPNETCORE_URLS=http://+:8080

RUN addgroup -g 2000 appgroup \
    && adduser -u 1000 -G appgroup -D "appuser"

RUN chown appuser:appgroup  /app
USER appuser:appgroup

COPY --from=publish /app/publish .
ENTRYPOINT ["./ingress-nginx-validate-jwt"]