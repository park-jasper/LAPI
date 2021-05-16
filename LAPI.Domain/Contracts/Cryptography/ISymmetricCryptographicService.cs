using LAPI.Domain.Model.Cryptography;

namespace LAPI.Domain.Contracts.Cryptography
{
    public interface ISymmetricCryptographicService
    {
        int KeyLength { get; }
        ICryptographicService Create(SymmetricKey key);
    }
}