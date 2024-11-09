namespace QueueManagementSystem.MVC.Models
{
    public class FingerprintState
    {
        public event EventHandler<AuthenticationResult>? AuthenticationCompleted;

        public void NotifyAuthenticationCompleted(AuthenticationResult result)
        {
            AuthenticationCompleted?.Invoke(this, result);
        }
    }
}
