using QueueManagementSystem.MVC.Models.Users;

namespace QueueManagementSystem.MVC.Models
{
    public class UserRolesModel
    {
        public int UserId { get; set; }
        public virtual SystemUsersModel User { get; set; } = null!;
        public int RoleId { get; set; }
        public UserRole Role { get; set; }
    }
}
