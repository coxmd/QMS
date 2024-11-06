namespace QueueManagementSystem.MVC.Models
{
    public class FingerprintErrorData
    {
        // <summary>
        /// Error message describing what went wrong during fingerprint capture or processing
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// UTC timestamp when the error occurred
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
