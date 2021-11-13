using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Communication.Initialization
{
    [TestClass]
    public class RegistrationTests : InitializationTestBase
    {
        [TestMethod]
        public async Task SuccessfulRegistration()
        {
            await Given(Streams)
                .And(Certificates)
                .And(NoTransportEncryption)

                .When(
                    ParallelExecutionOf(
                        TheServerAcceptsAClient,
                        TheClientRegistersWithTheServer))

                .Then(OnClientRegisterWasCalled)
                .And(TheReceivedClientRegistrationGuidIsCorrect)
                .And(TheReceivedClientRegistrationCertificateIsCorrect)
                .And(OnClientConnectedWasCalled)
                .And(TheReceivedClientConnectionGuidIsCorrect);
        }
    }
}