using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LAPI.Providers.Xamarin.WirelessP2P.Contracts;
using Mapster;
using Robotics.Mobile.Core.Bluetooth.LE;
using DeviceState = LAPI.Providers.Xamarin.WirelessP2P.Contracts.DeviceState;

namespace LAPI.Providers.Droid.WirelessP2P.Wrapper
{
    public class BleDevice : IBluetoothLowEnergyDevice
    {
        public IDevice Device { get; }

        public BleDevice(IDevice device)
        {
            Device = device;
        }

        public static IBluetoothLowEnergyDevice Wrap(IDevice device) => new BleDevice(device);

        public Guid Id => Device.ID;
        public string Name => Device.Name;
        public int Rssi => Device.Rssi;
        public object NativeDevice => Device.NativeDevice;
        public DeviceState State => (DeviceState) Device.State;
        public IEnumerable<IBluetoothLowEnergyService> Services => Device.Services.Select(BleService.Wrap);
        public Task DiscoverServicesAsync()
        {
            var tcs = new TaskCompletionSource<int>();

            void Discovered(object sender, EventArgs args)
            {
                Device.ServicesDiscovered -= Discovered;
                tcs.SetResult(0);
            }

            Device.ServicesDiscovered += Discovered;
            Device.DiscoverServices();
            return tcs.Task;
        }

        public bool Equals(IBluetoothLowEnergyDevice other)
        {
            return other is BleDevice otherDevice && Device == otherDevice.Device;
        }
    }
}