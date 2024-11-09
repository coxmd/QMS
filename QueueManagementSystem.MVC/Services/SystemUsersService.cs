using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.MVC.Components;
using QueueManagementSystem.MVC.Data;
using QueueManagementSystem.MVC.Models.Users;

namespace QueueManagementSystem.MVC.Services
{
    public class SystemUsersService
    {
        private readonly IDbContextFactory<QueueManagementSystemContext> _db;

        public SystemUsersService(IDbContextFactory<QueueManagementSystemContext> db)
        {
            _db = db;
        }
        public async Task<bool> AddUser(SystemUsersModel usersModel)
        {
            
            try
            {
                using var context = _db.CreateDbContext();
                context.SystemUsers.Add(usersModel);
                
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;

            }




        }
        public async Task<bool> EditUser(SystemUsersModel usersModel)
        {
            using var context = _db.CreateDbContext();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingUser = await context.SystemUsers
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == usersModel.Id);

                if (existingUser == null)
                    return false;

                // Remove existing roles
                context.UserRoles.RemoveRange(existingUser.UserRoles);

                // Update user properties
                existingUser.Surname = usersModel.Surname;
                existingUser.OtherNames = usersModel.OtherNames;
                existingUser.Username = usersModel.Username;
                existingUser.Password = usersModel.Password;
                existingUser.Active = usersModel.Active;

                // Add new roles
                existingUser.UserRoles = usersModel.UserRoles;

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> ResetPassword(SystemUsersModel usersModel)
        {
            try
            {
                using var context = _db.CreateDbContext();
                context.SystemUsers.Update(usersModel);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;

            }
        }
        public async Task<List<SystemUsersModel>> FetUsers()
        {
            using var context = _db.CreateDbContext();
            return await context.SystemUsers
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync();
        }
        public async Task<SystemUsersModel?> FetUserName(string userName)
        {
            using var context = _db.CreateDbContext();
            
            return await context.SystemUsers.FirstOrDefaultAsync(h=>h.Username == userName);
        }
    }
}
