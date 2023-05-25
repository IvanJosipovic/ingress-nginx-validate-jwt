FROM mcr.microsoft.com/dotnet/sdk:6.0-cbl-mariner@sha256:eb01915525626d7929b6ab9f76d99d2acd61af74b2c406c7be8a3f891a890771 AS build
WORKDIR /src
COPY ingress-nginx-validate-jwt.csproj .
RUN dotnet restore "ingress-nginx-validate-jwt.csproj"
COPY . .
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-cbl-mariner-distroless@sha256:ac0e0d11e8eb95dd77e0d86386c66a2d2be128243625662c379bb0a1e4a03f4e AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=publish /app/publish .
ENTRYPOINT ["./ingress-nginx-validate-jwt"]
