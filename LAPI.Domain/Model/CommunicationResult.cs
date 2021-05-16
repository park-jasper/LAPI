using System;

namespace LAPI.Domain.Model
{
    public class CommunicationResult
    {
        public bool Successful { get; set; }
        public Exception Exception { get; set; }

        public static CommunicationResult Failed => new CommunicationResult { Successful = false };

        public static CommunicationResult<TResult> FromResult<TResult>(TResult result)
        {
            return new CommunicationResult<TResult>
            {
                Successful = true,
                Result = result,
            };
        }
    }

    public class CommunicationResult<TResult> : CommunicationResult
    {
        public TResult Result { get; set; }
        public new static CommunicationResult<TResult> Failed => From(CommunicationResult.Failed);

        public static CommunicationResult<TResult> From(CommunicationResult commResult, TResult result = default)
        {
            return new CommunicationResult<TResult>
            {
                Successful = commResult.Successful,
                Exception = commResult.Exception,
                Result = result
            };
        }
    }
}