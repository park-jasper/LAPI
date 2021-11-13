using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Domain.Model;

namespace LAPI.Domain.Extensions
{
    public static class StreamExtensions
    {
        public const int Int32ByteSize = 4;

        private static CommunicationResult<TResult> From<TResult>(CommunicationResult result) =>
            CommunicationResult<TResult>.From(result);

        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, CancellationToken token)
        {
            return stream.ReadAsync(buffer, 0, buffer.Length, token);
        }

        public static Task WriteAsync(this Stream stream, byte[] buffer, CancellationToken token)
        {
            return stream.WriteAsync(buffer, 0, buffer.Length, token);
        }

        public static async Task<CommunicationResult> WriteSafelyAsync(
            this Stream stream,
            StreamWritable element,
            CancellationToken token,
            bool recycleTemporaryWriteBuffer = true)
        {
            try
            {
                await stream.WriteAsync(element.Buffer, 0, element.ContentLength, token);
                if (recycleTemporaryWriteBuffer)
                {
                    element.RecycleBuffer();
                }
                return new CommunicationResult()
                {
                    Successful = true,
                };
            }
            catch (Exception exc)
            {
                return new CommunicationResult()
                {
                    Successful = false,
                    Exception = exc,
                };
            }
        }

        public static async Task<CommunicationResult> WriteSafelyAsync(
            this Stream stream,
            StreamWritable first,
            StreamWritable second,
            CancellationToken token,
            bool recycle = true)
        {
            var combined = StreamWritable.Combine(first, second);
            var result = await stream.WriteSafelyAsync(combined, token);
            if (recycle)
            {
                first.RecycleBuffer();
                second.RecycleBuffer();
            }
            return result;
        }

        public static async Task<CommunicationResult> WriteSafelyAsync(
            this Stream stream,
            StreamWritable first,
            StreamWritable second,
            StreamWritable third,
            CancellationToken token,
            bool recycle = true)
        {
            var combined = StreamWritable.Combine(first, second, third);
            var result = await stream.WriteSafelyAsync(combined, token);
            if (recycle)
            {
                first.RecycleBuffer();
                second.RecycleBuffer();
                third.RecycleBuffer();
            }
            return result;
        }

        public static async Task<CommunicationResult<byte[]>> ReadSafelyAsync(this Stream stream, int length, CancellationToken token)
        {
            var buffer = new byte[length];
            try
            {
                var bytesReceived = await stream.ReadAsync(buffer, token);
                if (bytesReceived == length)
                {
                    return new CommunicationResult<byte[]>
                    {
                        Successful = true,
                        Result = buffer
                    };
                }
            }
            catch (Exception exc)
            {
                return new CommunicationResult<byte[]>
                {
                    Successful = false,
                    Exception = exc
                };
            }
            return CommunicationResult<byte[]>.Failed;
        }

        public static async Task<CommunicationResult<byte>> ReceiveByteSafelyAsync(this Stream stream, CancellationToken token)
        {
            var result = await stream.ReadSafelyAsync(1, token);
            if (result.Successful)
            {
                return new CommunicationResult<byte>
                {
                    Successful = true,
                    Result = result.Result[0],
                };
            }
            return CommunicationResult<byte>.From(result);
        }

        public static async Task<CommunicationResult<string>> ReceiveStringSafelyAsync(this Stream stream, CancellationToken token)
        {
            var lengthResult = await stream.ReceiveInt32SafelyAsync(token);
            if (lengthResult.Successful)
            {
                var textResult = await stream.ReadSafelyAsync(lengthResult.Result, token);
                if (textResult.Successful)
                {
                    return new CommunicationResult<string>
                    {
                        Successful = true,
                        Result = Encoding.UTF8.GetString(textResult.Result),
                    };
                }
                else
                {
                    return CommunicationResult<string>.From(textResult);
                }
            }
            else
            {
                return CommunicationResult<string>.From(lengthResult);
            }
        }

        public static async Task<CommunicationResult<int>> ReceiveInt32SafelyAsync(this Stream stream, CancellationToken token)
        {
            var result = await stream.ReadSafelyAsync(Int32ByteSize, token);
            if (result.Successful)
            {
                return new CommunicationResult<int>
                {
                    Successful = true,
                    Result = BitConverter.ToInt32(result.Result, 0)
                };
            }
            return From<int>(result);
        }

        public static async Task<CommunicationResult<Guid>> ReceiveGuidSafelyAsync(this Stream stream, CancellationToken token)
        {
            var result = await stream.ReadSafelyAsync(new Guid().ToByteArray().Length, token);
            if (result.Successful)
            {
                return new CommunicationResult<Guid>
                {
                    Successful = true,
                    Result = new Guid(result.Result)
                };
            }
            return From<Guid>(result);
        }

        public struct StreamWritable
        {
            public byte[] Buffer { get; private set; }
            public readonly int ContentLength;
            private bool recycleBuffer;

            public StreamWritable(byte[] buffer, int contentLength, bool recycleBuffer)
            {
                this.Buffer = buffer;
                this.ContentLength = contentLength;
                this.recycleBuffer = recycleBuffer;
            }

            public void RecycleBuffer()
            {
                if (!recycleBuffer)
                {
                    return;
                }
                
                BufferPool.Return(this.Buffer);

                this.Buffer = null;
                this.recycleBuffer = false;
            }
            
            private static readonly ArrayPool<byte> BufferPool = ArrayPool<byte>.Shared;

            public static implicit operator StreamWritable(byte[] buffer) =>
                new StreamWritable(buffer, buffer.Length, false);

            public static implicit operator StreamWritable(bool logical)
            {
                var buffer = BitConverter.GetBytes(logical);
                return new StreamWritable(buffer, buffer.Length, false);
            }

            public static implicit operator StreamWritable(int number)
            {
                var buffer = BufferPool.Rent(4);
                BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(), number);
                return new StreamWritable(buffer, 4, true);
            }

            public static implicit operator StreamWritable(long number)
            {
                var buffer = BufferPool.Rent(8);
                BinaryPrimitives.WriteInt64LittleEndian(buffer.AsSpan(), number);
                return new StreamWritable(buffer, 8, true);
            }

            public static implicit operator StreamWritable(float number)
            {
                var buffer = BitConverter.GetBytes(number);
                return new StreamWritable(buffer, buffer.Length, false);
            }

            public static implicit operator StreamWritable(double number)
            {
                var buffer = BitConverter.GetBytes(number);
                return new StreamWritable(buffer, buffer.Length, false);
            }

            public static implicit operator StreamWritable(Guid guid)
            {
                var buffer = guid.ToByteArray();
                return new StreamWritable(buffer, buffer.Length, false);
            }

            public static implicit operator StreamWritable(string text)
            {
                const int intLength = 4;
                int stringEncodedLength = Encoding.UTF8.GetByteCount(text);
                var buffer = BufferPool.Rent(intLength + stringEncodedLength);
                BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(), stringEncodedLength);
                Encoding.UTF8.GetBytes(text, 0, text.Length, buffer, intLength);
                return new StreamWritable(buffer, intLength + stringEncodedLength, true);
            }

            public static StreamWritable Combine(StreamWritable first, StreamWritable second)
            {
                var totalLength = first.ContentLength + second.ContentLength;
                if (first.Buffer.Length >= totalLength)
                {
                    Array.Copy(second.Buffer, 0, first.Buffer, first.ContentLength, second.ContentLength);
                    return new StreamWritable(first.Buffer, totalLength, false);
                }
                var result = BufferPool.Rent(totalLength);
                Array.Copy(first.Buffer, 0, result, 0, first.ContentLength);
                Array.Copy(second.Buffer, 0, result, first.ContentLength, second.ContentLength);
                return new StreamWritable(result, totalLength, true);
            }

            public static StreamWritable Combine(
                StreamWritable first,
                StreamWritable second,
                StreamWritable third)
            {
                var totalLength = first.ContentLength + second.ContentLength + third.ContentLength;
                if (first.Buffer.Length >= totalLength)
                {
                    Array.Copy(second.Buffer, 0, first.Buffer, first.ContentLength, second.ContentLength);
                    Array.Copy(third.Buffer, 0, first.Buffer, first.ContentLength + second.ContentLength, third.ContentLength);
                    return new StreamWritable(first.Buffer, totalLength, false);
                }

                var result = BufferPool.Rent(totalLength);
                Array.Copy(first.Buffer, 0, result, 0, first.ContentLength);
                Array.Copy(second.Buffer, 0, result, first.ContentLength, second.ContentLength);
                Array.Copy(third.Buffer, 0, result, first.ContentLength + second.ContentLength, third.ContentLength);
                return new StreamWritable(result, totalLength, true);
            }
        }
    }

}