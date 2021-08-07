using System;
using System.Collections.Generic;
using System.Linq;
using LAPI.Providers.Xamarin.WirelessP2P.Contracts;
using Mapster;
using Robotics.Mobile.Core.Bluetooth.LE;
using DeviceConnectionEventArgs = LAPI.Providers.Xamarin.WirelessP2P.Contracts.DeviceConnectionEventArgs;

namespace LAPI.Providers.Droid.WirelessP2P.Wrapper
{
    public class BleAdapter : IBluetoothLowEnergyAdapter
    {
        private readonly IAdapter _adapter;

        public BleAdapter(IAdapter adapter)
        {
            _adapter = adapter;

            _adapter.DeviceDiscovered += (sender, args) =>
                DeviceDiscovered?.Invoke(sender, BleDevice.Wrap(args.Device));
            _adapter.DeviceConnected += (sender, args) =>
                DeviceConnected?.Invoke(sender, args.Adapt<DeviceConnectionEventArgs>());
            _adapter.DeviceDisconnected += (sender, args) =>
                DeviceDisconnected?.Invoke(sender, args.Adapt<DeviceConnectionEventArgs>());

            _adapter.ScanTimeoutElapsed += (sender, args) => ScanTimeoutElapsed?.Invoke(sender, args);
        }

        public static IBluetoothLowEnergyAdapter Wrap(IAdapter adapter) => new BleAdapter(adapter);

        public bool IsScanning => _adapter.IsScanning;

        public IEnumerable<IBluetoothLowEnergyDevice> DiscoveredDevices =>
            _adapter.DiscoveredDevices.Select(BleDevice.Wrap);

        public IEnumerable<IBluetoothLowEnergyDevice> ConnectedDevices =>
            _adapter.ConnectedDevices.Select(BleDevice.Wrap);

        public event EventHandler<IBluetoothLowEnergyDevice> DeviceDiscovered;
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnected;
        public event EventHandler<DeviceConnectionEventArgs> DeviceDisconnected;
        public event EventHandler ScanTimeoutElapsed;

        public void ConnectToDevice(IBluetoothLowEnergyDevice device)
        {
            if (device is BleDevice wrap)
            {
                _adapter.ConnectToDevice(wrap.Device);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Only pass {nameof(IBluetoothLowEnergyDevice)} instances that were retrieved from the {nameof(DiscoveredDevices)} list");
            }
        }

        public void DisconnectDevice(IBluetoothLowEnergyDevice device)
        {
            if (device is BleDevice wrap)
            {
                _adapter.DisconnectDevice(wrap.Device);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Only pass {nameof(IBluetoothLowEnergyDevice)} instances that were retrieved from the {nameof(ConnectedDevices)} list");
            }
        }

        public void StartScanningForDevices() => _adapter.StartScanningForDevices();

        public void StartScanningForDevices(Guid serviceUuid) => _adapter.StartScanningForDevices(serviceUuid);

        public void StopScanningForDevices() => _adapter.StopScanningForDevices();
    }
}