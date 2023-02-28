FROM mcr.microsoft.com/dotnet/sdk:7.0.201-alpine3.17@sha256:0d7b6221bf1bd839432a991863956681761ac7a9c9dc78b4523ee3cef24ca6ab AS build
WORKDIR /src
COPY ["ingress-nginx-validate-jwt/ingress-nginx-validate-jwt.csproj", "ingress-nginx-validate-jwt/"]
RUN dotnet restore "ingress-nginx-validate-jwt/ingress-nginx-validate-jwt.csproj"
COPY . .
WORKDIR "/src/ingress-nginx-validate-jwt"
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime --self-contained true -o /app/publish /p:Version=${VERSION} /p:PublishTrimmed=false

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0.3-alpine3.17@sha256:941c37be76209272b5a5ff23b43f165a9ba1f855b9b9a0594fcba21dfc855020 AS base
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