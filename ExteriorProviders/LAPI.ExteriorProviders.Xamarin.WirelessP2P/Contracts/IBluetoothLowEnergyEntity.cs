using System;

namespace LAPI.Providers.Xamarin.WirelessP2P.Contracts
{
    public interface IBluetoothLowEnergyEntity
    {
        Guid Id { get; }
        string Name { get; }
    }
}