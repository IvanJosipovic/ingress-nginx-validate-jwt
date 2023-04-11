FROM mcr.microsoft.com/dotnet/sdk:6.0.407-alpine3.17@sha256:f83d8cf9f2b84935b078f0c18a73d8417b4f3fbe278a1a9e8ad7a9bf0c16c5d9 AS build
WORKDIR /src
COPY ingress-nginx-validate-jwt.csproj .
RUN dotnet restore "ingress-nginx-validate-jwt.csproj"
COPY . .
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime --self-contained true -o /app/publish /p:Version=${VERSION} /p:PublishTrimmed=false

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.16-alpine3.17@sha256:7426564d89e0b3cce8e722af272f46655e0f8370f02d921ea870e956f4874614 AS base
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
