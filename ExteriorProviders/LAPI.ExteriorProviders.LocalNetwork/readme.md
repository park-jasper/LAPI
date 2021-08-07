# LAPI.Providers.LocalNetwork

Contains an `IServer` implementation for communication via LAN/Wifi using `System.Net.Sockets.TcpListener`

Server:
```
var server = new Lapi.Providers.LocalNetwork.TcpServer(IPAddress.Loopback, 1234);
Lapi.RunServer(server, ...);
```

Client:
```
var client = new System.Net.Sockets.TcpClient(AddressFamily.InterNetwork);
await client.ConnectAsync(IPAddress.Loopback, 1234);
Lapi.RegisterWithServer(client, ...);
```