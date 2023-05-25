FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0-cbl-mariner@sha256:550993fcfd5e1b6854d60d9ec12e0a0960938cd3dc5cb4a1eed9b8d3eee9a059 AS build
WORKDIR /src
COPY ingress-nginx-validate-jwt.csproj .
RUN dotnet restore "ingress-nginx-validate-jwt.csproj"
COPY . .
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-cbl-mariner-distroless@sha256:fc59404586ca3aaa9ed30957a89282b27e67ecd8a8a5f80795db684f7db3729e AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=publish /app/publish .
ENTRYPOINT ["./ingress-nginx-validate-jwt"]
