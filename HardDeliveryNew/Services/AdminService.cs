using System.Threading.Tasks;
using HardDelivery.Models;
using Microsoft.AspNetCore.Identity;

namespace HardDelivery.Services
{
    public interface IAdminService
    {
        Task<bool> ChangeUserRoleAsync(string userId, string newRole);
    }

    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;

        public AdminService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ChangeUserRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
            await _userManager.AddToRoleAsync(user, newRole);

            return true;
        }
    }
}
