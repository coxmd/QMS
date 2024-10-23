using System.ComponentModel.DataAnnotations;

namespace QueueManagementSystem.MVC.Models.SmsConfig
{
    public class SmsSettingsModel
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string SenderId { get; set; }
    }
}
