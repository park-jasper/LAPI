using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LAPI.Providers.Xamarin.WirelessP2P.Contracts
{
    public interface IBluetoothLowEnergyService : IBluetoothLowEnergyEntity
    {
        bool IsPrimary { get; }
        IEnumerable<IBluetoothLowEnergyCharacteristic> Characteristics { get; }

        Task DiscoverCharacteristicsAsync();
        IBluetoothLowEnergyCharacteristic FindCharacteristic(string name, Guid id);
    }
}