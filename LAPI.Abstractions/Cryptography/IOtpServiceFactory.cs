namespace LAPI.Abstractions.Cryptography
{
    public interface IOtpServiceFactory
    {
        ICryptographicService GetCurrentOtp();
    }
}