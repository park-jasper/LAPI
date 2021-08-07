using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LAPI.Providers.Xamarin.WirelessP2P.Contracts
{
    public interface IBluetoothLowEnergyCharacteristic : IBluetoothLowEnergyEntity
    {
        string Uuid { get; }
        byte[] Value { get; }
        string StringValue { get; }
        IEnumerable<IBluetoothLowEnergyDescriptor> Descriptors { get; }
        object NativeCharacteristics { get; }
        CharacteristicPropertyType Properties { get; }
        bool CanRead { get; }
        bool CanUpdate { get; }
        bool CanWrite { get; }

        event EventHandler<IBluetoothLowEnergyCharacteristic> ValueUpdated;

        Task<IBluetoothLowEnergyCharacteristic> ReadAsync();
        void StartUpdates();
        void StopUpdates();
        void Write(byte[] data);
    }
}