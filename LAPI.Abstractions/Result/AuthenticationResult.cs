using System;
using System.Net.Security;

namespace LAPI.Abstractions.Result
{
    public abstract class AuthenticationResult
    {
        public sealed class Authenticated : AuthenticationResult
        {
            public Authenticated(AuthenticatedStream authenticatedStream)
            {
                AuthenticatedStream = authenticatedStream;
            }

            public AuthenticatedStream AuthenticatedStream { get; }
        }

        public sealed class AuthenticationFailed : AuthenticationResult
        {
            public AuthenticationFailed(Exception exception)
            {
                Exception = exception;
            }

            public Exception Exception { get; }
        }

        public sealed class TaskCanceled : AuthenticationResult
        {

        }

        public sealed class NotSupported : AuthenticationResult
        {
            public NotSupported(Exception exception)
            {
                Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}