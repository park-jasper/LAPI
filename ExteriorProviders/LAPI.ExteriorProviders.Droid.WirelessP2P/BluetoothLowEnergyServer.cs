using System;
using LAPI.Providers.Droid.WirelessP2P.Wrapper;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace LAPI.Providers.Droid.WirelessP2P
{
    public class BluetoothLowEnergyServer : ExteriorProviders.Xamarin.WirelessP2P.BluetoothLowEnergyServer
    {
        public BluetoothLowEnergyServer(IAdapter adapter) : base(BleAdapter.Wrap(adapter))
        {

        }
    }
}
