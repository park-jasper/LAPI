# LAPI.Providers.Aes

Contains a `LAPI.Abstractions.Cryptography.IOtpServiceFactory` implementation for OTP-encryption using AES-256

Server:
```
var server = new LapiServerBuilder()
    ...
    .WithAesOtpCryptography(() => GetCurrentOtp())
    ...
    .BuildServer();
```

Client:
```
var client = new LapiClientBuilder()
    ...
    .WithAesOtpCryptography(() => GetCurrentOtp())
    ...
    .BuildClient();
```
