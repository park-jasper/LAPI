using System;

namespace LAPI.Providers.Xamarin.WirelessP2P.Contracts
{
    public class DeviceConnectionEventArgs : EventArgs
    {
        public IBluetoothLowEnergyDevice Device;
        public string ErrorMessage;
    }
}