using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LAPI.Abstractions;
using LAPI.Providers.Xamarin.WirelessP2P;
using LAPI.Providers.Xamarin.WirelessP2P.Contracts;

namespace LAPI.ExteriorProviders.Xamarin.WirelessP2P
{
    public class BluetoothLowEnergyServer : IServer
    {
        private readonly IBluetoothLowEnergyAdapter _adapter;
        private readonly IList<IBluetoothLowEnergyDevice> _acceptedDevices = new List<IBluetoothLowEnergyDevice>();

        public BluetoothLowEnergyServer(IBluetoothLowEnergyAdapter adapter)
        {
            _adapter = adapter;
        }

        public Task<Stream> AcceptClientAsync()
        {
            var connectedButNotAcceptedDevice = _adapter.ConnectedDevices.Except(_acceptedDevices).FirstOrDefault();
            if (connectedButNotAcceptedDevice != null)
            {
                var stream = GetBleStream(connectedButNotAcceptedDevice);
                return Task.FromResult(stream);
            }

            var tcs = new TaskCompletionSource<Stream>();

            void Connected(object sender, DeviceConnectionEventArgs args)
            {
                _adapter.DeviceConnected -= Connected;
                tcs.SetResult(GetBleStream(args.Device));
            }

            _adapter.DeviceConnected += Connected;
            return tcs.Task;
        }

        private Stream GetBleStream(IBluetoothLowEnergyDevice device)
        {
            var stream = new BluetoothLowEnergyStream(device, () =>
            {
                _acceptedDevices.Remove(device);
                _adapter.DisconnectDevice(device);
            });
            _acceptedDevices.Add(device);
            return stream;
        }
    }
}