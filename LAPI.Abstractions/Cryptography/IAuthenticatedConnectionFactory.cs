using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Abstractions.Result;

namespace LAPI.Abstractions.Cryptography
{
    public interface IAuthenticatedConnectionFactory
    {
        Task<AuthenticationResult> AuthenticateAsServerAsync(Stream clientStream, CancellationToken cancellationToken);

        Task<AuthenticationResult> AuthenticateAsClientAsync(Stream serverStream, Guid assumedServerGuid, CancellationToken cancellationToken);
    }
}