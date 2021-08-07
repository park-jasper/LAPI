using System;

namespace LAPI.Providers.Xamarin.WirelessP2P.Contracts
{
    [Flags]
    public enum CharacteristicPropertyType
    {
        Broadcast = 1,
        Read = 2,
        AppleWriteWithoutResponse = 4,
        WriteWithoutResponse = 8,
        Notify = 16,
        Indicate = 32,
        AuthenticatedSignedWrites = 64,
        ExtendedProperties = 128,
        NotifyEncryptionRequired = 256,
        IndicateEncryptionRequired = 512
    }
}