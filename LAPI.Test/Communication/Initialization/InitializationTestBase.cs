using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CSharpToolbox.UnitTesting;
using FluentAssertions;
using LAPI.Cryptography;
using LAPI.Domain.Contracts.Cryptography;
using LAPI.Domain.Model.Cryptography;
using LAPI.Test.Mocks;

namespace LAPI.Test.Communication.Initialization
{
    public class InitializationTestBase : StreamTestBase
    {
        public static readonly Guid TestGuid = new Guid("ca9fc5a0-86d6-45d4-9133-714a512e19d9");
        public static readonly Guid OtherTestGuid = new Guid("08f281d2-9a7e-479e-9c37-25e8a97093ef");
        public const string Password = "SomeTestPasswordJustForTestingNothingToSeeHere";
        public static byte[] PSK = new byte[] { 1, 2, 3, 4 };
        public static ICryptographicService OTP = new CryptographicServiceMock();

        protected X509Certificate2 ClientCertificate;
        protected X509Certificate2 ServerCertificate;


        protected bool OnClientRegisteredCalled;
        protected Guid ClientRegisteredGuid;
        protected X509Certificate ClientRegisteredCertificate;

        protected bool OnClientConnectedCalled;
        protected Guid ClientConnectedGuid;
        protected AuthenticatedStream ClientConnectedStream;

        protected AuthenticatedStream ServerConnectedStream;

        protected void Certificates()
        {
            ServerCertificate = X509CertificateService.GetOwnCertificate(TestGuid, Password);
            ClientCertificate = X509CertificateService.GetOwnCertificate(OtherTestGuid, Password);
        }

        protected Task TheServerAcceptsAClient()
        {
            return Domain.Communication.Initialization.HandleInitializationOfClient(
                Client,
                PSK,
                TestGuid,
                ServerCertificate,
                OTP,
                (guid, stream) =>
                {
                    OnClientConnectedCalled = true;
                    ClientConnectedGuid = guid;
                    ClientConnectedStream = stream;
                },
                (guid, cert) =>
                {
                    OnClientRegisteredCalled = true;
                    ClientRegisteredGuid = guid;
                    ClientRegisteredCertificate = cert;
                },
                _ => ClientCertificate,
                Token);
        }

        protected async Task TheClientRegistersWithTheServer()
        {
            var result = await Domain.Communication.Initialization.RegisterWithServerAsync(
                Server,
                PSK,
                OtherTestGuid,
                TestGuid,
                ClientCertificate,
                ServerCertificate,
                OTP,
                CancellationToken.None);
            ServerConnectedStream = result.Result;
        }

        protected void OnClientRegisterWasCalled()
        {
            OnClientRegisteredCalled.Should().BeTrue();
        }

        protected void OnClientConnectedWasCalled()
        {
            OnClientConnectedCalled.Should().BeTrue();
        }

        protected void TheReceivedClientRegistrationGuidIsCorrect()
        {
            ClientRegisteredGuid.Should().Be(OtherTestGuid);
        }

        protected void TheReceivedClientRegistrationCertificateIsCorrect()
        {
            ClientRegisteredCertificate.Should().BeEquivalentTo(ClientCertificate);
        }

        protected void TheReceivedClientConnectionGuidIsCorrect()
        {
            ClientConnectedGuid.Should().Be(OtherTestGuid);
        }

        protected void TheStreamsShouldBeMutuallyAuthenticated()
        {
            ClientConnectedStream.IsMutuallyAuthenticated.Should().BeTrue();
            ServerConnectedStream.IsMutuallyAuthenticated.Should().BeTrue();
        }
    }
}