using System.Threading.Tasks;
using LAPI.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Extensions.IStreamExtensions.WorkingStreams
{
    [TestClass]
    public class Given_AClientStreamAndAServerStream : IStreamExtensionsBase
    {
        public override async Task Given()
        {
            await base.Given();

            IStreamMock.Create(out var server, out var client);
            Server = server;
            Client = client;
        }
    }
}