using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LAPI.Providers.Xamarin.WirelessP2P.Contracts;
using Robotics.Mobile.Core.Bluetooth.LE;
using CharacteristicPropertyType = LAPI.Providers.Xamarin.WirelessP2P.Contracts.CharacteristicPropertyType;

namespace LAPI.Providers.Droid.WirelessP2P.Wrapper
{
    public class BleCharacteristic : IBluetoothLowEnergyCharacteristic
    {
        private readonly ICharacteristic _characteristic;

        public BleCharacteristic(ICharacteristic characteristic)
        {
            _characteristic = characteristic;
            _characteristic.ValueUpdated += (sender, args) => ValueUpdated?.Invoke(sender, Wrap(args.Characteristic));
        }

        public static IBluetoothLowEnergyCharacteristic Wrap(ICharacteristic characteristic) =>
            new BleCharacteristic(characteristic);

        public Guid Id => _characteristic.ID;
        public string Name => _characteristic.Name;
        public string Uuid => _characteristic.Uuid;
        public byte[] Value => _characteristic.Value;
        public string StringValue => _characteristic.StringValue;
        public IEnumerable<IBluetoothLowEnergyDescriptor> Descriptors => _characteristic.Descriptors.Select(BleDescriptor.Wrap);
        public object NativeCharacteristics => _characteristic.NativeCharacteristic;
        public CharacteristicPropertyType Properties => (CharacteristicPropertyType) _characteristic.Properties;
        public bool CanRead => _characteristic.CanRead;
        public bool CanUpdate => _characteristic.CanUpdate;
        public bool CanWrite => _characteristic.CanWrite
        ;
        public event EventHandler<IBluetoothLowEnergyCharacteristic> ValueUpdated;
        public async Task<IBluetoothLowEnergyCharacteristic> ReadAsync()
        {
            var result = await _characteristic.ReadAsync();
            return Wrap(result);
        }

        public void StartUpdates() => _characteristic.StartUpdates();

        public void StopUpdates() => _characteristic.StopUpdates();

        public void Write(byte[] data) => _characteristic.Write(data);
    }
}