using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Providers.Xamarin.WirelessP2P.Contracts;

namespace LAPI.Providers.Xamarin.WirelessP2P
{
    public class BluetoothLowEnergyStream : Stream
    {
        private readonly IBluetoothLowEnergyDevice _device;
        private readonly Action _onDispose;
        private readonly Task _initializationTask;

        public BluetoothLowEnergyStream(IBluetoothLowEnergyDevice device, Action onDispose)
        {
            _device = device;
            _onDispose = onDispose;

            var tcs = new TaskCompletionSource<int>();
            _device.DiscoverServicesAsync().ContinueWith(async _ =>
            {
                await ServicesDiscovered();
                tcs.SetResult(0);
            });
            _initializationTask = tcs.Task;
        }

        private async Task ServicesDiscovered()
        {
            await Task.WhenAll(_device.Services.Select(s => s.DiscoverCharacteristicsAsync()));
            foreach (var characteristic in _device.Services.SelectMany(s => s.Characteristics))
            {
                //characteristic.Properties
            }
        }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _onDispose?.Invoke();
            }
        }
    }
}