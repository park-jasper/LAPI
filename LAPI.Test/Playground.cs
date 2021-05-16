using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.X509;

namespace LAPI.Test
{
    [TestClass]
    public class Playground
    {

        [TestMethod]
        public void Test()
        {
            int p = 5;
            var someGuid = Guid.NewGuid();
            var text = someGuid.ToString();
            var gen = new X509V3CertificateGenerator();
            var supported = string.Join(", ", Asn1SignatureFactory.SignatureAlgNames);
        }
    }
}