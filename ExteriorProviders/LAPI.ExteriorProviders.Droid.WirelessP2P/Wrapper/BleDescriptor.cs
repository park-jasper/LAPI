using System;
using LAPI.Providers.Xamarin.WirelessP2P.Contracts;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace LAPI.Providers.Droid.WirelessP2P.Wrapper
{
    public class BleDescriptor : IBluetoothLowEnergyDescriptor
    {
        private readonly IDescriptor _descriptor;

        public BleDescriptor(IDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public static IBluetoothLowEnergyDescriptor Wrap(IDescriptor descriptor) => new BleDescriptor(descriptor);

        public Guid Id => _descriptor.ID;
        public string Name => _descriptor.Name;
        public object NativeDescriptor => _descriptor.NativeDescriptor;
    }
}