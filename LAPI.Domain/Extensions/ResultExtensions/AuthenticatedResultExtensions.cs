using System;
using System.Threading.Tasks;
using LAPI.Abstractions.Result;

namespace LAPI.Domain.Extensions.ResultExtensions
{
    public static class AuthenticatedResultExtensions
    {
        public static void Match(
            this AuthenticationResult result,
            Action<AuthenticationResult.Authenticated> authenticated,
            Action<AuthenticationResult.AuthenticationFailed> authenticationFailed,
            Action<AuthenticationResult.NotSupported> notSupported,
            Action<AuthenticationResult.TaskCanceled> taskCanceled)
        {
            result.TypeMatch(authenticated, authenticationFailed, notSupported, taskCanceled);
        }

        public static TMatchResult Match<TMatchResult>(
            this AuthenticationResult result,
            Func<AuthenticationResult.Authenticated, TMatchResult> authenticated,
            Func<AuthenticationResult.AuthenticationFailed, TMatchResult> authenticationFailed,
            Func<AuthenticationResult.NotSupported, TMatchResult> notSupported,
            Func<AuthenticationResult.TaskCanceled, TMatchResult> taskCanceled)
        {
            return result.TypeMatch(authenticated, authenticationFailed, notSupported, taskCanceled);
        }

        public static Task<TMatchResult> Match<TMatchResult>(
            this AuthenticationResult result,
            Func<AuthenticationResult.Authenticated, Task<TMatchResult>> authenticated,
            Func<AuthenticationResult.AuthenticationFailed, Task<TMatchResult>> authenticationFailed,
            Func<AuthenticationResult.NotSupported, Task<TMatchResult>> notSupported,
            Func<AuthenticationResult.TaskCanceled, Task<TMatchResult>> taskCanceled)
        {
            return result.TypeMatch(authenticated, authenticationFailed, notSupported, taskCanceled);
        }
    }
}