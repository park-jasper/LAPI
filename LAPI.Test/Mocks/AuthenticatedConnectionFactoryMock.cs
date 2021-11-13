using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Abstractions.Cryptography;
using LAPI.Abstractions.Result;

namespace LAPI.Test.Mocks
{
    public class AuthenticatedConnectionFactoryMock : IAuthenticatedConnectionFactory
    {
        public Task<AuthenticationResult> AuthenticateAsServerAsync(Stream clientStream, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                (AuthenticationResult) new AuthenticationResult.Authenticated(
                    new AuthenticatedStreamMock(clientStream, false, true)));
        }

        public Task<AuthenticationResult> AuthenticateAsClientAsync(Stream serverStream, Guid assumedServerGuid, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                (AuthenticationResult) new AuthenticationResult.Authenticated(
                    new AuthenticatedStreamMock(serverStream, false, false)));
        }
    }
}