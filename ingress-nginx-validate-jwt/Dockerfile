FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ingress-nginx-validate-jwt/ingress-nginx-validate-jwt.csproj", "ingress-nginx-validate-jwt/"]
RUN dotnet restore "ingress-nginx-validate-jwt/ingress-nginx-validate-jwt.csproj"
COPY . .
WORKDIR "/src/ingress-nginx-validate-jwt"
RUN dotnet build "ingress-nginx-validate-jwt.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "ingress-nginx-validate-jwt.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:Version=${VERSION}

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ingress-nginx-validate-jwt.dll"]