using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Providers.Xamarin.WirelessP2P.Contracts;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace LAPI.Providers.Droid.WirelessP2P.Wrapper
{
    public class BleService : IBluetoothLowEnergyService
    {
        private readonly IService _service;

        public BleService(IService service)
        {
            _service = service;
        }

        public static IBluetoothLowEnergyService Wrap(IService service) => new BleService(service);
        public Guid Id => _service.ID;
        public string Name => _service.Name;
        public bool IsPrimary => _service.IsPrimary;
        public IEnumerable<IBluetoothLowEnergyCharacteristic> Characteristics => _service.Characteristics.Select(BleCharacteristic.Wrap);

        public Task DiscoverCharacteristicsAsync()
        {
            var tcs = new TaskCompletionSource<int>();

            void Discovered(object sender, EventArgs args)
            {
                tcs.SetResult(0);
                _service.CharacteristicsDiscovered -= Discovered;
            }

            _service.CharacteristicsDiscovered += Discovered;
            _service.DiscoverCharacteristics();
            return tcs.Task;
        }

        public IBluetoothLowEnergyCharacteristic FindCharacteristic(string name, Guid id)
        {
            return BleCharacteristic.Wrap(
                _service.FindCharacteristic(new KnownCharacteristic() {ID = id, Name = name}));
        }
    }
}