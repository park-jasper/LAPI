using Org.BouncyCastle.Crypto;
using System;
using System.Linq;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;

namespace LAPI.Model.Cryptography
{
    public class RsaCryptographicService : ICryptographicService
    {
        //private RsaPrivateKey _privateKey;
        //private RsaPublicKey _publicKey;
        private readonly IAsymmetricBlockCipher _rsa;
        private readonly ICipherParameters _privateKey;
        private readonly ICipherParameters _publicKey;

        public RsaCryptographicService(RsaKeyPair keyPair)
        {
            //_rsa = new RSACng();
            _rsa = new OaepEncoding(new RsaEngine(), new Sha512Digest());
            //var kp = DotNetUtilities.GetRsaKeyPair(new RSAParameters
            //{
            //    Modulus = keyPair.PrivateKey.Modulus.ToArray(),
            //    Exponent = keyPair.PublicKey.PublicKeyExponent.ToArray(),
            //    D = keyPair.PrivateKey.PrivateKeyExponent.ToArray(),
            //});
            //_privateKey = kp.Private;
            //_publicKey = kp.Public;
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(new RSAParameters
            {
                Modulus = keyPair.PrivateKey.Modulus.ToArray(),
                Exponent = keyPair.PublicKey.PublicKeyExponent.ToArray(),
                D = keyPair.PrivateKey.PrivateKeyExponent.ToArray(),
            });
            var kp = DotNetUtilities.GetRsaKeyPair(csp);
            _privateKey = kp.Private;
            _publicKey = kp.Public;
            CanDecrypt = true;
        }

        public RsaCryptographicService(RsaPublicKey publicKey)
        {
            //_rsa = new RSACng();
            _rsa = new OaepEncoding(new RsaEngine(), new Sha512Digest());
            _publicKey = DotNetUtilities.GetRsaPublicKey(new RSAParameters
            {
                Modulus = publicKey.Modulus.ToArray(),
                Exponent = publicKey.PublicKeyExponent.ToArray(),
            });
            //_rsa = new RSACryptoServiceProvider();
            //_rsa.ImportParameters(new RSAParameters
            //{
            //    Modulus = publicKey.Modulus.ToArray(),
            //    Exponent = publicKey.PublicKeyExponent.ToArray(),
            //});
            CanDecrypt = false;
        }

        public byte[] Encrypt(byte[] content)
        {
            _rsa.Init(true, _publicKey);
            return _rsa.ProcessBlock(content, 0, content.Length);
            //return _rsa.Encrypt(content, RSAEncryptionPadding.OaepSHA512);
        }

        public byte[] Decrypt(byte[] cipher)
        {
            if (!CanDecrypt)
            {
                throw new InvalidOperationException("This Cryptographic Service cannot decrypt");
            }
            _rsa.Init(false, _privateKey);
            return _rsa.ProcessBlock(cipher, 0, cipher.Length);
            //return _rsa.Decrypt(cipher, RSAEncryptionPadding.OaepSHA512);
        }

        public bool CanEncrypt => true;
        public bool CanDecrypt { get; }
    }
}