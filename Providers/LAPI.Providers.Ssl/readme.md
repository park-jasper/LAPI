# LAPI.Providers.Ssl

Contains a `LAPI.Abstractions.Cryptography.IAuthenticatedConnectionFactory` implementation to enable secure communication via SSL

Server:
```
var server = new LapiServerBuilder()
    ...
    .WithSslEncryption(serverCertificate)
    ...
    .BuildServer();
```

Client:
```
var client = new LapiClientBuilder()
    ...
    .WithSslEncryption(clientCertificate)
    ...
    .BuildClient();
```