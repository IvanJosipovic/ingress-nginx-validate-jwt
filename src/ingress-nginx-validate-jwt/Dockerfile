ARG BUILDPLATFORM=linux/amd64

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine@sha256:1659a7a350b7847f6afc84df1409c6b80fd08e7decf51f5c365303b0d625f13b AS build
WORKDIR /src
COPY ingress-nginx-validate-jwt.csproj .
RUN dotnet restore "ingress-nginx-validate-jwt.csproj"
COPY . .
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine@sha256:ae971060544904dc25f344269d874043ac057462dec77bf70eb26b86a53087c7 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# renovate: datasource=repology depName=alpine_3_19/ca-certificates-bundle versioning=loose
ENV CA_CERTIFICATES_BUNDLES_VERSION="20241121-r1"

# renovate: datasource=repology depName=alpine_3_19/busybox versioning=loose
ENV BUSYBOX_VERSION="1.36.1-r19"

# renovate: datasource=repology depName=alpine_3_19/busybox-binsh versioning=loose
ENV BUSYBOX_BINSH_VERSION="1.36.1-r19"

# renovate: datasource=repology depName=alpine_3_19/libc-utils versioning=loose
ENV LIBC_UTILS_VERSION="0.7.2-r5"

# renovate: datasource=repology depName=alpine_3_19/libcrypto3 versioning=loose
ENV LIBCRYPTO3_VERSION="3.1.8-r0"

# renovate: datasource=repology depName=alpine_3_19/ssl_client versioning=loose
ENV SSL_VERSION="1.36.1-r19"

# renovate: datasource=repology depName=alpine_3_19/libgcc versioning=loose
ENV LIBGCC_VERSION="13.2.1_git20231014-r0"

# renovate: datasource=repology depName=alpine_3_19/libssl3 versioning=loose
ENV LIBSSL3_VERSION="3.1.8-r0"

# renovate: datasource=repology depName=alpine_3_19/libstdc++ versioning=loose
ENV LIBSTDC_VERSION="13.2.1_git20231014-r0"

# renovate: datasource=repology depName=alpine_3_19/zlib versioning=loose
ENV ZLIB_VERSION="1.3.1-r0"

RUN apk add --upgrade --no-cache \
        ca-certificates-bundle>$CA_CERTIFICATES_BUNDLES_VERSION \
        busybox>$BUSYBOX_BINSH_VERSION \
        busybox-binsh>$BUSYBOX_BINSH_VERSION \
        libc-utils>$LIBC_UTILS_VERSION \
        libcrypto3>$LIBCRYPTO3_VERSION \
        ssl_client>$SSL_VERSION \
        \
        # .NET dependencies
        libgcc>$LIBGCC_VERSION \
        libssl3>$LIBSSL3_VERSION \
        libstdc++>$LIBSTDC_VERSION \
        zlib>$ZLIB_VERSION

USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["./ingress-nginx-validate-jwt"]
