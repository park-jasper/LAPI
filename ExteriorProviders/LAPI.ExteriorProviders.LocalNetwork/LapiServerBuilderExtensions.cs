using System.Net;
using LAPI.Domain.Builder;

namespace LAPI.ExteriorProviders.LocalNetwork
{
    public static class LapiServerBuilderExtensions
    {
        public static LapiServerBuilder WithTcpServer(this LapiServerBuilder builder, IPAddress localIpAddress, int port)
        {
            return builder.WithCustomServerImplementation(() => new TcpServer(localIpAddress, port));
        }
    }
}