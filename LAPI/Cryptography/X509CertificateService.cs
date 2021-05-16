using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using X509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate;

namespace LAPI.Cryptography
{
    public class X509CertificateService
    {
        private static string GetStorePath(Guid ownIdentity) =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LAPI", $"{ownIdentity}.cer");
        public static X509Certificate2 GetOwnCertificate(Guid ownIdentity, string password, bool createIfNotExists = true)
        {
            var ownStore = new FileInfo(GetStorePath(ownIdentity));
            if (ownStore.Exists)
            {
                return new X509Certificate2(ownStore.FullName, password);
            }
            else
            {
                return GenerateAndVerifyCertificate(ownIdentity, password);
            }
        }
        public static X509Certificate2 GenerateAndVerifyCertificate(Guid ownIdentity, string password)
        {
            var certificate = CreateX509Certificate(ownIdentity, password);
            return certificate;
        }
        public static X509Certificate2 CreateX509Certificate(Guid ownIdentity, string password)
        {
            var keyPairGenerator = new RsaKeyPairGenerator();
            var generationParameters =
                new KeyGenerationParameters(
                    new SecureRandom(new CryptoApiRandomGenerator()),
                    2048);
            keyPairGenerator.Init(generationParameters);

            var keyPair = keyPairGenerator.GenerateKeyPair();


            var certificateGenerator = new X509V3CertificateGenerator();
            var certificateName = new X509Name("CN=" + ownIdentity.ToString());
            var random = new SecureRandom(new CryptoApiRandomGenerator());
            var serialNumber = BigInteger.ProbablePrime(120, random);

            certificateGenerator.SetSerialNumber(serialNumber);
            certificateGenerator.SetSubjectDN(certificateName);
            certificateGenerator.SetIssuerDN(certificateName);
            certificateGenerator.SetNotAfter(DateTime.Now.AddYears(1));
            certificateGenerator.SetNotBefore(DateTime.Now);
            certificateGenerator.SetPublicKey(keyPair.Public);

            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id, 
                false,
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public),
                    new GeneralNames(new GeneralName(certificateName)), 
                    serialNumber)
                );
            //certificateGenerator.AddExtension(
            //    X509Extensions.ExtendedKeyUsage.Id,
            //    false,
            //    new ExtendedKeyUsage(KeyPurposeID.AnyExtendedKeyUsage));

            var signatureFactory = new Asn1SignatureFactory("SHA512WITHRSA", keyPair.Private);
            var certificate = certificateGenerator.Generate(signatureFactory);
            return ToCertificateWithPrivateKey(ownIdentity, certificate, keyPair, password, random);
        }

        public static X509Certificate2 ToCertificateWithPrivateKey(
            Guid ownIdentity, 
            Org.BouncyCastle.X509.X509Certificate certificate,
            AsymmetricCipherKeyPair keyPair, 
            string password, 
            SecureRandom random)
        {
            var ownCertificateStore = new FileInfo(GetStorePath(ownIdentity));

            var newStore = new Pkcs12Store();
            var certEntry = new X509CertificateEntry(certificate);

            newStore.SetCertificateEntry(
                ownIdentity.ToString(),
                certEntry);
            newStore.SetKeyEntry(
                ownIdentity.ToString(),
                new AsymmetricKeyEntry(keyPair.Private),
                new[] { certEntry });
            if (ownCertificateStore.Exists)
            {
                ownCertificateStore.Delete();
            }
            using (var file = ownCertificateStore.OpenWrite())
            {
                newStore.Save(
                    file, 
                    password.ToCharArray(),
                    random);
            }

            return new X509Certificate2(ownCertificateStore.FullName, password);
        }

        public static X509Certificate GetRemoteCertificate(Guid remoteGuid)
        {
            var remoteCertificateStore = new FileInfo(GetStorePath(remoteGuid));
            if (remoteCertificateStore.Exists)
            {
                var cert = new X509Certificate();
                cert.Import(remoteCertificateStore.FullName);
                return new X509Certificate(remoteCertificateStore.FullName, "");
            }
            else
            {
                return null;
            }
        }
        public static void SaveRemoteCertificate(Guid remoteGuid, X509Certificate certificate)
        {
            var remoteCertificateStore = new FileInfo(GetStorePath(remoteGuid));
            if (remoteCertificateStore.Exists)
            {
                remoteCertificateStore.Delete();
            }
            var content = certificate.Export(X509ContentType.Cert);
            using (var file = remoteCertificateStore.OpenWrite())
            {
                file.Write(content, 0, content.Length);
            }
        }
    }
}