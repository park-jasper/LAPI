using System;
using LAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Model.Cryptography
{
    public class Given_AnAesCryptographicServiceWithValidKey : TestSpecs
    {
        protected AesCryptographicService Aes;

        public override void Given()
        {
            Aes = new AesCryptographicService(TestData.TestSymmetricKey);
        }
    }
}