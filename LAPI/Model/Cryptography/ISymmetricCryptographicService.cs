namespace LAPI.Model.Cryptography
{
    public interface ISymmetricCryptographicService
    {
        int KeyLength { get; }
        ICryptographicService Create(SymmetricKey key);
    }
}