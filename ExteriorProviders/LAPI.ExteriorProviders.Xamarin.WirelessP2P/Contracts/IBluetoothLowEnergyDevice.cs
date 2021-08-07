using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LAPI.Providers.Xamarin.WirelessP2P.Contracts
{
    public interface IBluetoothLowEnergyDevice : IBluetoothLowEnergyEntity, IEquatable<IBluetoothLowEnergyDevice>
    {
        int Rssi { get; }
        object NativeDevice { get; }
        DeviceState State { get; }
        IEnumerable<IBluetoothLowEnergyService> Services { get; }

        Task DiscoverServicesAsync();
    }
}