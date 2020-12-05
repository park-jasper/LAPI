namespace LAPI.Model.Cryptography
{
    public interface IAsymmetricCryptographicService
    {
        ICryptographicService Create(RsaPublicKey key);
        ICryptographicService Create(RsaKeyPair keyPair);
    }
}