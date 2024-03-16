ARG BUILDPLATFORM=linux/amd64

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine@sha256:b141e243684bd341229c0f9cddf185adffda918f423398fbc67a84fd676fd3d8 AS build
WORKDIR /src
COPY ingress-nginx-validate-jwt.csproj .
RUN dotnet restore "ingress-nginx-validate-jwt.csproj"
COPY . .
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine@sha256:ddda904d7c75a25fd29d757fe6ee39ff85090a96ac7a3d0e3f1d7b74f691e821 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# renovate: datasource=repology depName=alpine_3_18/ca-certificates-bundle versioning=loose
ENV CA_CERTIFICATES_BUNDLES_VERSION="20230506-r0"

# renovate: datasource=repology depName=alpine_3_18/libgcc versioning=loose
ENV LIBGCC_VERSION="12.2.1_git20220924-r10"

# renovate: datasource=repology depName=alpine_3_18/libssl3 versioning=loose
ENV LIBSSL3_VERSION="3.1.4-r5"

# renovate: datasource=repology depName=alpine_3_18/libstdc++ versioning=loose
ENV LIBSTDC_VERSION="12.2.1_git20220924-r10"

# renovate: datasource=repology depName=alpine_3_18/zlib versioning=loose
ENV ZLIB_VERSION="1.2.13-r1"

RUN apk add --upgrade --no-cache \
        ca-certificates-bundle>$CA_CERTIFICATES_BUNDLES_VERSION \
        \
        # .NET dependencies
        libgcc>$LIBGCC_VERSION \
        libssl3>$LIBSSL3_VERSION \
        libstdc++>$LIBSTDC_VERSION \
        zlib>$ZLIB_VERSION

USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["./ingress-nginx-validate-jwt"]
