using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpToolbox.UnitTesting;
using FluentAssertions;
using LAPI.Domain.Extensions;
using LAPI.Domain.Model;
using LAPI.Test.Mocks;

namespace LAPI.Test.Communication
{
    public abstract class StreamTestBase : GivenWhenThen
    {
        protected Stream Client { get; set; }
        protected Stream Server { get; set; }
        protected CancellationToken Token => CancellationToken.None;
        protected CommunicationResult<byte[]> ReadResult;

        protected void Streams()
        {
            StreamMock.Create(out var server, out var client);
            Server = server;
            Client = client;
        }

        protected AsyncAction TheClientSendsBytes(params byte[] bytes)
        {
            return () => Client.WriteAsync(bytes, Token);
        }

        protected AsyncAction TheServerReadsBytesSafely(int count)
        {
            return async () =>
            {
                ReadResult = await Server.ReadSafelyAsync(count, Token);
            };
        }

        protected void TheReadIsSuccessful()
        {
            ReadResult.Successful.Should().Be(true);
        }
        protected Action TheResultIs(params byte[] bytes)
        {
            return () => ReadResult.Result.Should().BeEquivalentTo(bytes);
        }

        protected AsyncAction ParallelExecutionOf(params Func<Task>[] tasks)
        {
            return () => Task.WhenAll(tasks.Select(Task.Run));
        }
    }
}