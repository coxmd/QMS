using System.ComponentModel.DataAnnotations;

namespace QueueManagementSystem.MVC.Models.Users
{
    public class SystemUsersModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Surname { get; set; }
        [Required]
        public string? OtherNames { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        public bool Active { get; set; } = true;
        // Add navigation property for roles
        public virtual ICollection<UserRolesModel> UserRoles { get; set; } = new List<UserRolesModel>();
    }
}
