namespace QueueManagementSystem.MVC.Models
{
    public class FingerprintCallbackData
    {
        /// <summary>
        /// Base64 encoded string of the captured fingerprint data
        /// </summary>
        public string FingerprintData { get; set; }

        /// <summary>
        /// UTC timestamp when the fingerprint was captured
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
