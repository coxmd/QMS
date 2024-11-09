using QueueManagementSystem.MVC.Models;

namespace QueueManagementSystem.MVC.Services
{
    public interface IFingerprintService
    {
        Task<bool> InitializeAsync();
        Task<bool> OpenDeviceAsync();
        Task<AuthenticationResult> EnrollAsync();
        Task<AuthenticationResult> SearchIdnoAsync(CustomerInfo customer);
        Task<AuthenticationResult> ContinuousScanAsync(CustomerInfo customer);
        Task<AuthenticationResult> MatchFingerPrintAsync(CustomerInfo customer);
        Task<AuthenticationResult> AuthenticateAsync();
        Task CloseDeviceAsync();
        Task<int> GetDeviceCountAsync();
    }
}
