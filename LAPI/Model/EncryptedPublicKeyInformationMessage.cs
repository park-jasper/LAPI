namespace LAPI.Model
{
    public readonly struct EncryptedPublicKeyInformationMessage
    {
        public int EncryptedCombinationLength { get; }
        public int DecryptedModulusLength { get; }
        public byte ExponentPadding { get; }

        public EncryptedPublicKeyInformationMessage(
            int encryptedCombinationLength,
            int decryptedModulusLength,
            byte exponentPadding)
        {
            EncryptedCombinationLength = encryptedCombinationLength;
            DecryptedModulusLength = decryptedModulusLength;
            ExponentPadding = exponentPadding;
        }
    }
}