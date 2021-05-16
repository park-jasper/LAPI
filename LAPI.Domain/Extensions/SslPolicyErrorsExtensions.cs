using System.Net.Security;

namespace LAPI.Domain.Extensions
{
    public static class SslPolicyErrorsExtensions
    {
        public static bool HasFlag(this SslPolicyErrors errors, SslPolicyErrors flag)
        {
            return (errors & flag) == flag;
        }
    }
}