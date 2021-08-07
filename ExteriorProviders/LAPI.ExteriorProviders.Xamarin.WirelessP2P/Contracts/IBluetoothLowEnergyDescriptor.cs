namespace LAPI.Providers.Xamarin.WirelessP2P.Contracts
{
    public interface IBluetoothLowEnergyDescriptor : IBluetoothLowEnergyEntity
    {
        object NativeDescriptor { get; }
    }
}