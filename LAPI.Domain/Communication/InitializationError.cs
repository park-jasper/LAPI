
namespace LAPI.Domain.Communication
{
    public class InitializationError
    {
        public InitializationErrorType ErrorType { get; set; }
        public string Message { get; set; }
    }

    public enum InitializationErrorType
    {
        CancellationRequested,
        Communication,
        Authentication,
        Identification,
        Protocol
    }
}