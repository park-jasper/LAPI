using LAPI.Domain.Model;

namespace LAPI.Domain.Communication
{
    public class InitializationResult
    {
        public bool Successful { get; set; }
        public InitializationError Error { get; set; }

        public static InitializationResult Failed => new InitializationResult { Successful = false };

        public static InitializationResult From(CommunicationResult cErr)
        {
            return new InitializationResult
            {
                Successful = false,
                Error = new InitializationError
                {
                    ErrorType = InitializationErrorType.Communication,
                    Message = cErr.Exception?.Message,
                }
            };
        }
    }
    public class InitializationResult<TResult> : InitializationResult
    {
        public TResult Result { get; set; }
        public new static InitializationResult<TResult> Failed => From(InitializationResult.Failed);

        public static InitializationResult<TResult> From(InitializationResult commResult, TResult result = default)
        {
            return new InitializationResult<TResult>
            {
                Successful = commResult.Successful,
                Error = commResult.Error,
                Result = result
            };
        }
    }
}