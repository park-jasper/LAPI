using LAPI.Model.Cryptography;

namespace LAPI.Test.Model.Cryptography
{
    public class Given_AnRsaCryptographicServiceWithAValidKeyPair : TestSpecs
    {
        protected RsaCryptographicService Rsa;
        public override void Given()
        {
            base.Given();
            Rsa = new RsaCryptographicService(TestData.TestAsymmetricKeyPair);
        }
    }
}