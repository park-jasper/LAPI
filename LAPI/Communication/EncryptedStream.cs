using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Contracts;
using LAPI.Extensions;
using LAPI.Model;

namespace LAPI.Communication
{
    public class EncryptedStream : IStream
    {
        private readonly ICryptographicService _service;
        private readonly IStream _stream;
        private int _currentBufferPosition;
        private byte[] _buffer;

        public EncryptedStream(ICryptographicService service, IStream stream)
        {
            _service = service;
            _stream = stream;
        }
        public async Task<int> ReadAsync(byte[] buffer, int count, CancellationToken token)
        {
            if (_buffer != null)
            {
                var remainingBufferBytes = _buffer.Length - _currentBufferPosition;
                var readFromBuffer = Math.Min(remainingBufferBytes, count);
                Array.Copy(_buffer, _currentBufferPosition, buffer, 0, readFromBuffer);
                _currentBufferPosition += readFromBuffer;
                if (_currentBufferPosition == _buffer.Length)
                {
                    _buffer = null;
                    _currentBufferPosition = 0;
                }
                return readFromBuffer;
            }
            if (!_service.CanDecrypt)
            {
                throw new InvalidOperationException("cannot decrypt stream");
            }
            if (count < 0)
            {
                throw new ArgumentException("cannot read less than 0 bytes");
            }
            if (count == 0)
            {
                return 0;
            }
            var lengthResult = await _stream.ReceiveInt32SafelyAsync(token);
            if (!lengthResult.Successful)
            {
                throw lengthResult.Exception;
            }
            var length = lengthResult.Result;
            if (length < 0)
            {
                throw new ProtocolException("length < 0");
            }
            var cipherResult = await _stream.ReadSafelyAsync(length, token);
            if (!cipherResult.Successful)
            {
                throw cipherResult.Exception;
            }
            var plaintext = _service.Decrypt(cipherResult.Result);
            if (count < plaintext.Length)
            {
                _buffer = plaintext;
                _currentBufferPosition = count;
            }
            var readFromPlaintext = Math.Min(plaintext.Length, count);
            Array.Copy(plaintext, 0, buffer, 0, readFromPlaintext);
            return readFromPlaintext;
        }

        public async Task WriteAsync(byte[] buffer, int count, CancellationToken token)
        {
            if (!_service.CanEncrypt)
            {
                throw new InvalidOperationException("cannot encrypt stream");
            }
            var encrypted = _service.Encrypt(buffer);
            var result = await _stream.WriteSafelyAsync(token, encrypted.Length, encrypted);
            if (!result.Successful)
            {
                throw result.Exception;
            }
        }
    }
}