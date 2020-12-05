using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Contracts;
using LAPI.Extensions;
using LAPI.Model;
using LAPI.Model.Cryptography;
using LAPI.Model.Exceptions;

namespace LAPI.Communication
{
    public class Initialization
    {
        private static CommunicationResult<TResult> From<TResult>(CommunicationResult result) =>
            CommunicationResult<TResult>.From(result);
        public static async Task<CommunicationResult> HandleInitializationOfClient(
            IStream clientStream,
            byte[] presharedKey,
            Guid serverGuid,
            ICryptographicService otp,
            IAsymmetricCryptographicService asymmetric,
            ISymmetricCryptographicService symmetric,
            Action<IStream> onClientConnected,
            Action<Guid, RsaPublicKey> onClientRegistered,
            Func<Guid, RsaPublicKey> getClientPublicKey,
            CancellationToken token)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            token.Register(timeoutCancellationTokenSource.Cancel);
            if (token.IsCancellationRequested)
            {
                return CommunicationResult.Failed;
            }
            timeoutCancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
            token = timeoutCancellationTokenSource.Token;
            if (!await GetAndVerifyPresharedKey(clientStream, presharedKey, token))
            {
                return CommunicationResult.Failed;
            }
            await clientStream.WriteSafelyAsync(token, serverGuid);
            var clientGuidResult = await clientStream.ReceiveGuidSafelyAsync(token);
            if (!clientGuidResult.Successful)
            {
                return clientGuidResult;
            }
            var clientGuid = clientGuidResult.Result;
            var initiationModeResult = await clientStream.ReceiveInt32SafelyAsync(token);
            if (!initiationModeResult.Successful)
            {
                return initiationModeResult;
            }
            switch ((InitiationMode) initiationModeResult.Result)
            {
                case InitiationMode.Otp:
                    var clientRegistrationResult =
                        await HandleClientRegistrationSafelyAsync(clientStream, otp, token);
                    if (clientRegistrationResult.Successful)
                    {
                        onClientRegistered(clientGuid, clientRegistrationResult.Result);
                    }
                    else
                    {
                        return clientRegistrationResult;
                    }
                    return new CommunicationResult
                    {
                        Successful = true,
                    };
                case InitiationMode.Standard:
                    var clientPublicKey = getClientPublicKey(clientGuid);
                    if (clientPublicKey == null)
                    {
                        return CommunicationResult.Failed;
                    }
                    var sessionKey = SymmetricKey.GenerateNewKey(symmetric.KeyLength);
                    var sendSessionKeyResult = await clientStream.WriteSafelyAsync(token,
                        asymmetric.Create(clientPublicKey).Encrypt(sessionKey.Key.ToArray()));
                    if (!sendSessionKeyResult.Successful)
                    {
                        return sendSessionKeyResult;
                    }
                    onClientConnected(new EncryptedStream(symmetric.Create(sessionKey), clientStream));
                    return new CommunicationResult
                    {
                        Successful = true,
                    };
                default:
                    throw new ProtocolException($"invalid initiation mode {initiationModeResult.Result}");
            }
        }

        public static async Task<bool> GetAndVerifyPresharedKey(
            IStream clientStream, 
            byte[] presharedKey,
            CancellationToken token)
        {
            var result = await clientStream.ReadSafelyAsync(presharedKey.Length, token);
            if (result.Successful)
            {
                var clientPsk = result.Result;
                if (clientPsk.Length != presharedKey.Length)
                {
                    return false;
                }
                return clientPsk.StartsWith(presharedKey);
            }
            return false;
        }

        public static async Task<CommunicationResult<RsaPublicKey>> HandleClientRegistrationSafelyAsync(
            IStream clientStream,
            ICryptographicService otp, 
            CancellationToken token)
        {
            CommunicationResult<RsaPublicKey> From(CommunicationResult result) => From<RsaPublicKey>(result);
            var Failed = From(CommunicationResult.Failed);

            if (!otp.CanDecrypt)
            {
                throw new ArgumentException("otp needs to be able to decrypt");
            }

            var dataTypeResult = await clientStream.ReceiveInt32SafelyAsync(token);
            if (!dataTypeResult.Successful)
            {
                return From(dataTypeResult);
            }
            if ((CommunicationData) dataTypeResult.Result != CommunicationData.PublicKey)
            {
                return Failed;
            }

            var getKeyLengthResult = await GetEncryptedPublicKeyLengths(clientStream, token);
            if (!getKeyLengthResult.Successful)
            {
                return From(getKeyLengthResult);
            }
            var pubKeyInfo = getKeyLengthResult.Result;
            var encryptedCombinationResult =
                await clientStream.ReadSafelyAsync(pubKeyInfo.EncryptedCombinationLength, token);
            if (!encryptedCombinationResult.Successful)
            {
                return From(encryptedCombinationResult);
            }
            var encryptedCombination = encryptedCombinationResult.Result;
            var decryptedCombination = otp.Decrypt(encryptedCombination);
            var modulus = new byte[pubKeyInfo.DecryptedModulusLength];
            var decryptedExponentLength = decryptedCombination.Length - pubKeyInfo.DecryptedModulusLength - pubKeyInfo.ExponentPadding;
            var exponent = new byte[decryptedExponentLength];
            Array.Copy(decryptedCombination, 0, modulus, 0, pubKeyInfo.DecryptedModulusLength);
            Array.Copy(decryptedCombination, pubKeyInfo.DecryptedModulusLength, exponent, 0, decryptedExponentLength);
            var clientPublicKey = new RsaPublicKey(modulus, exponent);
            return new CommunicationResult<RsaPublicKey>
            {
                Successful = true,
                Result = clientPublicKey,
            };
        }

        public static async Task<CommunicationResult<EncryptedPublicKeyInformationMessage>> GetEncryptedPublicKeyLengths(
            IStream clientStream, 
            CancellationToken token)
        {
            var result = await clientStream.ReadSafelyAsync(2 * Size.Int + Size.Byte, token);
            if (!result.Successful)
            {
                return From<EncryptedPublicKeyInformationMessage>(result);
            }
            var buffer = result.Result;
            var first = BitConverter.ToInt32(buffer, 0);
            var second = BitConverter.ToInt32(buffer, Size.Int);
            var third = buffer[2 * Size.Int];
            return new CommunicationResult<EncryptedPublicKeyInformationMessage>()
            {
                Successful = true,
                Result = new EncryptedPublicKeyInformationMessage(first, second, third),
            };
        }

        public static async Task<CommunicationResult> HandleServerDiscoveryMessage(
            byte[] request,
            IPEndPoint remoteEndPoint,
            byte[] presharedKey,
            Guid serverGuid,
            CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return CommunicationResult.Failed;
            }
            if (!request.StartsWith(presharedKey))
            {
                return CommunicationResult.Failed;
            }
            var client = new UdpClient(remoteEndPoint.Port, remoteEndPoint.AddressFamily);
            var datagram = serverGuid.ToByteArray();
            try
            {
                await client.SendAsync(datagram, datagram.Length, remoteEndPoint);
            }
            catch (Exception exc)
            {
                return new CommunicationResult()
                {
                    Successful = false,
                    Exception = exc,
                };
            }
            return new CommunicationResult()
            {
                Successful = true,
            };
        }
    }
}