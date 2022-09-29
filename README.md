# ingress-nginx-validate-jwt

[![codecov](https://codecov.io/gh/IvanJosipovic/ingress-nginx-validate-jwt/branch/main/graph/badge.svg?token=hh1FWYrH5r)](https://codecov.io/gh/IvanJosipovic/ingress-nginx-validate-jwt)

## What is this?

This project is an API server which is used along with the [nginx.ingress.kubernetes.io/auth-url](https://github.com/kubernetes/ingress-nginx/blob/main/docs/user-guide/nginx-configuration/annotations.md#external-authentication) annotation for ingress-nginx and enables per Ingress customizable JWT validation.

## Install

```bash
helm repo add ingress-nginx-validate-jwt https://ivanjosipovic.github.io/ingress-nginx-validate-jwt

helm repo update

helm install ingress-nginx-validate-jwt ingress-nginx-validate-jwt/ingress-nginx-validate-jwt --devel --create-namespace --namespace ingress-nginx-validate-jwt --set openIdProviderConfigurationUrl="https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration"
```

### Options

- openIdProviderConfigurationUrl
  - OpenID Provider Configuration Url for your Identity Provider
- logLevel
  - Logging Level (Trace, Debug, Information, Warning, Error, Critical, and None)
- [Helm Values](charts/ingress-nginx-validate-jwt/values.yaml)

## Configure Ingress

```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: ingress
  namespace: default
  annotations:
    nginx.ingress.kubernetes.io/auth-url: http://ingress-nginx-validate-jwt.ingress-nginx-validate-jwt.svc.cluster.local/auth?tid=11111111-1111-1111-1111-111111111111&aud=22222222-2222-2222-2222-222222222222&aud=33333333-3333-3333-3333-333333333333
spec:
```

## Parameters

The /auth endpoint supports configurable parameters in the format of {claim}={value}. In the case the same claim is called more than once, the traffic will have to match only one.

For example, using the following query string
/auth?  
tid=11111111-1111-1111-1111-111111111111  
&aud=22222222-2222-2222-2222-222222222222  
&aud=33333333-3333-3333-3333-333333333333  

Along with validating the JWT token, the token must have a claim tid=11111111-1111-1111-1111-111111111111 and one of aud=22222222-2222-2222-2222-222222222222
 or aud=33333333-3333-3333-3333-333333333333

## Design

![alt text](/docs/validate-jwt.png)