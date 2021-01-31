using LAPI.Test.Mocks;

namespace LAPI.Test.Extensions.IStreamExtensions.Given
{
    public class AClientStreamAndAServerStream : IGiven<IStreamTestBase>
    {
        public void Given(IStreamTestBase tbase)
        {
            StreamMock.Create(out var server, out var client);
            tbase.Server = server;
            tbase.Client = client;
        }
    }
}