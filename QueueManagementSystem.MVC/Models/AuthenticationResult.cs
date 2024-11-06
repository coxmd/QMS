namespace QueueManagementSystem.MVC.Models
{
    public class AuthenticationResult
    {
        /// <summary>
        /// Indicates whether the fingerprint was successfully authenticated against existing records
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Additional message providing context about the authentication result
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional customer ID if the fingerprint matched an existing record
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Timestamp of when the authentication was completed
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
