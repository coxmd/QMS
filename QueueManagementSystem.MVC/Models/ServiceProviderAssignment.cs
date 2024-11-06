using QueueManagementSystem.MVC.Models.Users;

namespace QueueManagementSystem.MVC.Models
{
    public class ServiceProviderAssignment
    {
        public int Id { get; set; }
        public int SystemUserId { get; set; }
        public SystemUsersModel SystemUser { get; set; }
        public int ServicePointId { get; set; }
        public DateTime AssignmentTime { get; set; }
        public DateTime? SignInTime { get; set; }
        public DateTime? SignOutTime { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public ServicePoint ServicePoint { get; set; }
    }
}
