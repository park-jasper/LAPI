using System;
using CSharpToolbox.UnitTesting;
using FluentAssertions;
using LAPI.Domain.Extensions;
using LAPI.Domain.Model;

namespace LAPI.Test.Extensions.StreamExtensions
{
    public class ByteArrayResultBase : StreamExtensionsTestBase
    {
        protected override CommunicationResult Result 
        { 
            get => ByteArrayResult;
            set => ByteArrayResult = CommunicationResult<byte[]>.From(value);
        }
        protected CommunicationResult<byte[]> ByteArrayResult;

        protected AsyncAction TheClientReadsBytesSafely(int count)
        {
            return async () =>
            {
                ByteArrayResult = await Client.ReadSafelyAsync(count, Token);
            };
        }
        protected AsyncAction TheServerReadsBytesSafely(int count)
        {
            return async () =>
            {
                ByteArrayResult = await Server.ReadSafelyAsync(count, Token);
            };
        }

        protected Action TheResultIs(params byte[] bytes)
        {
            return () => ByteArrayResult.Result.Should().BeEquivalentTo(bytes);
        }
    }
}