# LAPI - Local Application Programming Interface

! this is a work in progress !

There are a lot of cloud-based solutions for a lot of different things, synchronizing your data, sending files, and so forth.
<br>
But more often than not, I would prefer a solution that can work without a server sitting somewhere in the cloud. In a lot of use-cases, my devices sit in the same network, or are physically close to one another.
<br>
While working on a cloudless synchronization app, I realized that this communication part better have its own project, because it was getting rather sizy.
<br>
Have you devices communicate directly with one another, e.g. over the local network (see [LAPI.Providers.LocalNetwork](https://github.com/JMarianczuk/LAPI/tree/master/LAPI.Providers.LocalNetwork)).

### Example
Server:
``` cs --source-file ./LAPI.TestApplication/Samples/ServerSample.cs --project ./LAPI.TestApplication/LAPI.TestApplication.csproj
```

Client:
``` cs --source-file ./LAPI.TestApplication/Samples/ClientSample.cs --project ./LAPI.TestApplication/LAPI.TestApplication.csproj
```

### Features
* Register clients with a server using an OTP, ideally transmitted out-of-band (e.g. using a QR-Code)
* Built-in authentication to prevent connections from unknown devices
* Establish an encrypted connection via SSL using X509Certificates
* Discover devices on your local network via UDP broadcast, so you are not forced to stick to a certain TCP port

### Planned future work
* More local area communication implementations, like
  * Windows Pipes
  * Wifi direct / bluetooth