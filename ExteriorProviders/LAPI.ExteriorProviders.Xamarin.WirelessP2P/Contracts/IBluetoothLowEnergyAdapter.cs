using System;
using System.Collections;
using System.Collections.Generic;

namespace LAPI.Providers.Xamarin.WirelessP2P.Contracts
{
    public interface IBluetoothLowEnergyAdapter
    {
        bool IsScanning { get; }
        IEnumerable<IBluetoothLowEnergyDevice> DiscoveredDevices { get; }
        IEnumerable<IBluetoothLowEnergyDevice> ConnectedDevices { get; }

        event EventHandler<IBluetoothLowEnergyDevice> DeviceDiscovered;
        event EventHandler<DeviceConnectionEventArgs> DeviceConnected;
        event EventHandler<DeviceConnectionEventArgs> DeviceDisconnected;
        event EventHandler ScanTimeoutElapsed;

        void ConnectToDevice(IBluetoothLowEnergyDevice device);
        void DisconnectDevice(IBluetoothLowEnergyDevice device);
        void StartScanningForDevices();
        void StartScanningForDevices(Guid serviceUuid);
        void StopScanningForDevices();
    }
}