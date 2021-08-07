namespace LAPI.Abstractions.Cryptography
{
    public interface ICryptographicService
    {
        byte[] Encrypt(byte[] content);
        byte[] Decrypt(byte[] cipher);
        bool CanEncrypt { get; }
        bool CanDecrypt { get; }
    }
}