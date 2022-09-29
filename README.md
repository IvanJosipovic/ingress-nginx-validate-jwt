# ingress-nginx-validate-jwt

### What is this?
This project is an API server which is used along with the [nginx.ingress.kubernetes.io/auth-url](https://github.com/kubernetes/ingress-nginx/blob/main/docs/user-guide/nginx-configuration/annotations.md#external-authentication) annotation for ingress-nginx and enables customizable JWT validation.

### Install

```bash
helm repo add ingress-nginx-validate-jwt https://ivanjosipovic.github.io/ingress-nginx-validate-jwt

helm repo update

helm template ingress-nginx-validate-jwt ingress-nginx-validate-jwt/ingress-nginx-validate-jwt --set openIdProviderConfigurationUrl="https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration"
```

Options

- openIdProviderConfigurationUrl
  - OpenID Provider Configuration Url for your Identity Provider
- logLevel
  - Logging Level (Trace, Debug, Information, Warning, Error, Critical, and None )
- [Helm Values](charts/ingress-nginx-validate-jwt/values.yaml)

### Configure Ingress

```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: ingress
  namespace: default
  annotations:
    nginx.ingress.kubernetes.io/auth-url: http://ingress-nginx-validate-jwt.ingress-nginx-validate-jwt.svc.cluster.local/auth?tid=47c63aee-0b28-46b6-b8fa-1cfb273e761b&aud=f2b06abf-ba7f-4fb9-8b14-d38d0d13d607&aud=17d5386c-b52a-4a0d-bf2a-89540495b39c
spec:
```

### Parameters

The /auth endpoint supports configurable parameters in the format of {claim}={value}. In the case the same claim is called more than once, the traffic will have to match only one.

For example, using the following query string /auth?tid=47c63aee-0b28-46b6-b8fa-1cfb273e761b&aud=f2b06abf-ba7f-4fb9-8b14-d38d0d13d607&aud=17d5386c-b52a-4a0d-bf2a-89540495b39c

Along with validating the JWT token, only traffic which have matching tid and aud(either of the two) claims will be authorized.
