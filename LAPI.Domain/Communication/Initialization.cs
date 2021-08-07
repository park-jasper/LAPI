using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CSharpToolbox.Extensions;
using LAPI.Abstractions.Cryptography;
using LAPI.Abstractions.Result;
using LAPI.Domain.Contracts;
using LAPI.Domain.Contracts.Cryptography;
using LAPI.Domain.Extensions;
using LAPI.Domain.Extensions.ResultExtensions;
using LAPI.Domain.Model;
using Newtonsoft.Json;

namespace LAPI.Domain.Communication
{
    public class Initialization
    {
        private static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);
        //private static TimeSpan DefaultTimeout = TimeSpan.FromMinutes(5);
        private static InitializationResult<TResult> From<TResult>(CommunicationResult result) =>
            InitializationResult<TResult>.From(InitializationResult.From(result));

        public static async Task<InitializationResult> HandleInitializationOfClient(
            Stream clientStream,
            byte[] presharedKey,
            Guid serverGuid,
            IAuthenticatedConnectionFactory authenticatedConnectionFactory,
            ICryptographicService otp,
            Action<Guid, AuthenticatedStream> onClientConnected,
            Action<Guid, X509Certificate> onClientRegistered,
            Func<Guid, X509Certificate> getClientPublicKey,
            CancellationToken token)
        {
            token = token.AddTimeout(DefaultTimeout);
            if (token.IsCancellationRequested)
            {
                return new InitializationResult
                {
                    Successful = false,
                    Error = new InitializationError
                    {
                        ErrorType = InitializationErrorType.CancellationRequested,
                    }
                };
            }
            if (!await GetAndVerifyPresharedKey(clientStream, presharedKey, token))
            {
                return new InitializationResult
                {
                    Successful = false,
                    Error = new InitializationError
                    {
                        ErrorType = InitializationErrorType.Protocol,
                        Message = "The received pre-shared key did not match the pre-shared key for this application",
                    },
                };
            }
            await clientStream.WriteSafelyAsync(serverGuid, token);
            var clientGuidResult = await clientStream.ReceiveGuidSafelyAsync(token);
            if (!clientGuidResult.Successful)
            {
                return InitializationResult.From(clientGuidResult);
            }
            var clientGuid = clientGuidResult.Result;
            var initiationModeResult = await clientStream.ReceiveInt32SafelyAsync(token);
            if (!initiationModeResult.Successful)
            {
                return InitializationResult.From(initiationModeResult);
            }
            switch ((InitiationMode) initiationModeResult.Result)
            {
                case InitiationMode.Otp:
                    var clientRegistrationResult =
                        await HandleClientRegistrationSafelyAsync(clientStream, serverGuid, authenticatedConnectionFactory, otp, token);
                    if (clientRegistrationResult.Successful)
                    {
                        var (certificate, stream) = clientRegistrationResult.Result;
                        onClientRegistered(clientGuid, certificate);
                        onClientConnected(clientGuid, stream);
                    }
                    else
                    {
                        return clientRegistrationResult;
                    }
                    return new InitializationResult
                    {
                        Successful = true,
                    };
                case InitiationMode.Standard:
                    var clientCertificate = getClientPublicKey(clientGuid);
                    if (clientCertificate == null)
                    {
                        return InitializationResult.Failed;
                    }
                    //var sessionKey = SymmetricKey.GenerateNewKey(symmetric.KeyLength);

                    var encryptedStreamResult = await EstablishEncryptedCommunication(true, serverGuid, authenticatedConnectionFactory, clientStream, token);
                    if (!encryptedStreamResult.Successful)
                    {
                        return encryptedStreamResult;
                    }
                    onClientConnected(clientGuid, encryptedStreamResult.Result);
                    return new InitializationResult
                    {
                        Successful = true,
                    };
                case InitiationMode.None:
                    return new InitializationResult()
                    {
                        Successful = false,
                        Error = new InitializationError()
                        {
                            ErrorType = InitializationErrorType.Protocol,
                            Message = "Client did not send a valid initiation mode",
                        },
                    };
                default:
                    throw new ProtocolException($"invalid initiation mode {initiationModeResult.Result}");
            }
        }

        public static async Task<bool> GetAndVerifyPresharedKey(
            Stream clientStream, 
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

        public static async Task<InitializationResult<(X509Certificate,AuthenticatedStream)>> HandleClientRegistrationSafelyAsync(
            Stream clientStream,
            Guid serverGuid,
            IAuthenticatedConnectionFactory authenticatedConnectionFactory,
            ICryptographicService otp,
            CancellationToken token)
        {
            InitializationResult<(X509Certificate,AuthenticatedStream)> From(CommunicationResult res) => From<(X509Certificate, AuthenticatedStream)>(res);

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
                return new InitializationResult<(X509Certificate, AuthenticatedStream)>
                {
                    Successful = false,
                    Error = new InitializationError
                    {
                        ErrorType = InitializationErrorType.Protocol,
                        Message = $"Client sent unexpected data",
                    },
                };
            }

            var encryptedLengthResult = await clientStream.ReceiveInt32SafelyAsync(token);
            if (!encryptedLengthResult.Successful)
            {
                return From(encryptedLengthResult);
            }
            var encryptedCertificateResult = await clientStream.ReadSafelyAsync(encryptedLengthResult.Result, token);
            if (!encryptedCertificateResult.Successful)
            {
                return From(encryptedCertificateResult);
            }
            var clientCertificate = new X509Certificate(otp.Decrypt(encryptedCertificateResult.Result));

            var authenticationResult = await EstablishEncryptedCommunication(true, serverGuid, authenticatedConnectionFactory, clientStream, token);
            if (authenticationResult.Successful)
            {
                return new InitializationResult<(X509Certificate, AuthenticatedStream)>
                {
                    Successful = true,
                    Result = (clientCertificate, authenticationResult.Result),
                };
            }
            else
            {
                return InitializationResult<(X509Certificate, AuthenticatedStream)>.From(authenticationResult);
            }
        }

        public static async Task<CommunicationResult> HandleServerDiscoveryMessage(
            byte[] request,
            Func<byte[], int, Task> sendAnswerAsync,
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
            var datagram = serverGuid.ToByteArray();
            try
            {
                await sendAnswerAsync(datagram, datagram.Length);
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

        public static async Task<InitializationResult<AuthenticatedStream>> EstablishEncryptedCommunication(
            bool asServer,
            Guid serverGuid,
            IAuthenticatedConnectionFactory authenticatedConnectionFactory,
            Stream stream,
            CancellationToken token)
        {
            async Task<CommunicationResult<string>> NotifyRemotePartyAndValidateRemotePartySuccess(string message)
            {
                CommunicationResult<string> remoteResult;
                CommunicationResult writeResult;
                if (asServer)
                {
                    remoteResult = await stream.ReceiveStringSafelyAsync(token);
                    writeResult = await stream.WriteSafelyAsync(message, token);
                }
                else
                {
                    writeResult = await stream.WriteSafelyAsync(message, token);
                    remoteResult = await stream.ReceiveStringSafelyAsync(token);
                }
                if (!writeResult.Successful && remoteResult.Successful)
                {
                    return CommunicationResult<string>.From(writeResult);
                }
                return remoteResult;
            }
            Task<CommunicationResult> NotifyRemoteParty(string message)
            {
                return stream.WriteSafelyAsync(message, token);
            }
            AuthenticationResult authenticationResult;
            if (asServer)
            {
                authenticationResult = await authenticatedConnectionFactory.AuthenticateAsServerAsync(stream, token);
            }
            else
            {
                authenticationResult = await authenticatedConnectionFactory.AuthenticateAsClientAsync(stream, serverGuid, token);
            }

            var result = await authenticationResult.Match(
                async authenticated =>
                {
                    var successfulInitResult = new InitializationResult
                    {
                        Successful = true,
                    };
                    var remoteInitResult = await NotifyRemotePartyAndValidateRemotePartySuccess(JsonConvert.SerializeObject(successfulInitResult));
                    if (remoteInitResult.Successful)
                    {
                        var remoteInitializationResult = JsonConvert.DeserializeObject<InitializationResult>(remoteInitResult.Result);
                        if (remoteInitializationResult.Successful)
                        {
                            return new InitializationResult<AuthenticatedStream>
                            {
                                Successful = true,
                                Result = authenticated.AuthenticatedStream,
                            };
                        }
                        else
                        {
                            var self = asServer ? "server" : "client";
                            var remote = asServer ? "client" : "server";
                            return new InitializationResult<AuthenticatedStream>
                            {
                                Successful = false,
                                Error = new InitializationError
                                {
                                    ErrorType = InitializationErrorType.Authentication,
                                    Message = $"The remote {remote} encountered a problem authentication this {self}. Their message: '{remoteInitializationResult.Error.Message}'",
                                },
                            };
                        }
                    }
                    else
                    {
                        return From<AuthenticatedStream>(remoteInitResult);
                    }
                },
                async authenticationFailed =>
                {
                    var failedInitResult = new InitializationResult
                    {
                        Successful = false,
                        Error = new InitializationError
                        {
                            ErrorType = InitializationErrorType.Authentication,
                            Message = authenticationFailed.Exception.Message,
                        },
                    };
                    await NotifyRemoteParty(JsonConvert.SerializeObject(failedInitResult));
                    return InitializationResult<AuthenticatedStream>.From(failedInitResult);
                },
                async notSupported =>
                {
                    var failedInitResult = new InitializationResult
                    {
                        Successful = false,
                        Error = new InitializationError
                        {
                            ErrorType = InitializationErrorType.Authentication,
                            Message = notSupported.Exception.Message,
                        }
                    };
                    await NotifyRemoteParty(JsonConvert.SerializeObject(failedInitResult));
                    return InitializationResult<AuthenticatedStream>.From(failedInitResult);
                },
                taskCanceled =>
                {
                    var failedInitResult = new InitializationResult<AuthenticatedStream>
                    {
                        Successful = false,
                        Error = new InitializationError
                        {
                            ErrorType = InitializationErrorType.CancellationRequested,
                        },
                    };
                    return Task.FromResult(failedInitResult);
                });

            return result;
        }

        public static async Task<InitializationResult<AuthenticatedStream>> RegisterWithServerAsync(
            Stream serverStream,
            byte[] presharedKey,
            Guid ownGuid,
            Guid serverGuid,
            IAuthenticatedConnectionFactory authenticatedConnectionFactory,
            X509Certificate2 clientCertificate,
            X509Certificate serverCertificate,
            ICryptographicService otp,
            CancellationToken token)
        {
            InitializationResult<AuthenticatedStream> From(CommunicationResult res) => From<AuthenticatedStream>(res);
            if (!otp.CanEncrypt)
            {
                throw new ArgumentException("otp needs to be able to encrypt");
            }
            token = token.AddTimeout(DefaultTimeout);

            await serverStream.WriteSafelyAsync(presharedKey, token);
            var serverGuidResult = await serverStream.ReceiveGuidSafelyAsync(token);
            if (!serverGuidResult.Successful)
            {
                return From(serverGuidResult);
            }
            if (!serverGuidResult.Result.Equals(serverGuid))
            {
                return new InitializationResult<AuthenticatedStream>
                {
                    Successful = false,
                    Error = new InitializationError
                    {
                        ErrorType = InitializationErrorType.Identification,
                        Message = $"Expected server to be '{serverGuid}', but instead found '{serverGuidResult.Result}'",
                    },
                };
            }
            await serverStream.WriteSafelyAsync(
                ownGuid, 
                (int) InitiationMode.Otp,
                token);

            var exportCertificate = clientCertificate.Export(X509ContentType.Cert);
            var encryptedCertificate = otp.Encrypt(exportCertificate);
            await serverStream.WriteSafelyAsync(
                (int) CommunicationData.PublicKey,
                encryptedCertificate.Length,
                encryptedCertificate,
                token);

            return await EstablishEncryptedCommunication(false, serverGuid, authenticatedConnectionFactory,
                serverStream, token);
        }

        public static async Task<InitializationResult<AuthenticatedStream>> ConnectToServerAsync(
            Stream serverStream,
            byte[] presharedKey,
            Guid ownGuid,
            Guid serverGuid,
            IAuthenticatedConnectionFactory authenticatedConnectionFactory,
            X509Certificate serverCertificate)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            timeoutCancellationTokenSource.CancelAfter(DefaultTimeout);
            var token = timeoutCancellationTokenSource.Token;

            await serverStream.WriteSafelyAsync(presharedKey, token);
            var serverGuidResult = await serverStream.ReceiveGuidSafelyAsync(token);
            if (!serverGuidResult.Successful)
            {
                return From<AuthenticatedStream>(serverGuidResult);
            }
            if (!serverGuidResult.Result.Equals(serverGuid))
            {
                return new InitializationResult<AuthenticatedStream>
                {
                    Successful = false,
                    Error = new InitializationError
                    {
                        ErrorType = InitializationErrorType.Identification,
                        Message = $"Expected server to be '{serverGuid}', but instead found '{serverGuidResult.Result}'",
                    },
                };
            }
            await serverStream.WriteSafelyAsync(
                ownGuid, 
                (int) InitiationMode.Standard,
                token);

            var encryptedStreamResult = await EstablishEncryptedCommunication(false, serverGuid, authenticatedConnectionFactory,
                serverStream, token);
            return encryptedStreamResult;
        }
    }
}