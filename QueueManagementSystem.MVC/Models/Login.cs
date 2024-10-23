using System.ComponentModel.DataAnnotations;

namespace QueueManagementSystem.MVC.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email {get; set;}

        [Required(ErrorMessage = "Please enter a password")]
        public string Password {get; set;}
    }
    
}