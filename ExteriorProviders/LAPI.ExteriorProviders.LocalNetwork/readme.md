# LAPI.ExteriorProviders.LocalNetwork

Contains an `IServer` implementation for communication via LAN/Wifi using `System.Net.Sockets.TcpListener`

Server:
```
var server = LapiServerBuilder()
    ...
    .WithTcpServer(ipAddress, port)
    ...
    .BuildServer();
Lapi.RunServer(server, ...);
```

Client:
```
var client = new System.Net.Sockets.TcpClient(AddressFamily.InterNetwork);
await client.ConnectAsync(ipAddress, port);
Lapi.RegisterWithServer(client, ...);
```