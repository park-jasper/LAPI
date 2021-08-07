using System;
using System.IO;
using System.Threading;
using CSharpToolbox.UnitTesting;
using FluentAssertions;
using LAPI.Domain.Extensions;
using LAPI.Domain.Model;
using LAPI.Test.Mocks;

namespace LAPI.Test.Extensions.StreamExtensions
{
    public abstract class StreamExtensionsTestBase : GivenWhenThen
    {
        protected Stream Client;
        protected Stream Server;
        protected CancellationToken Token = CancellationToken.None;
        protected abstract CommunicationResult Result { get; set; }

        protected void AClientStreamAndAServerStream()
        {
            StreamMock.Create(out var server, out var client);
            Client = client;
            Server = server;
        }

        protected void AClientStreamThatThrowsAnException()
        {
            Client = new MalfunctioningStream();
        }

        protected AsyncAction TheClientSendsBytesSafely(params byte[] bytes)
        {
            return async () =>
            {
                Result = await Client.WriteSafelyAsync(bytes, Token);
            };
        }

        protected void TheResultIsSuccessful()
        {
            Result.Successful.Should().BeTrue();
        }
        protected void TheResultIsNotSuccessful()
        {
            Result.Successful.Should().BeFalse();
        }
    }
}