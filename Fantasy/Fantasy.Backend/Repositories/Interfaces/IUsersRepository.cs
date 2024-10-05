using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Microsoft.AspNetCore.Identity;

namespace Fantasy.Backend.Repositories.Interfaces;

public interface IUsersRepository
{
    Task<string> GeneratePasswordResetTokenAsync(User user);

    Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

    Task<IdentityResult> UpdateUserAsync(User user);

    Task<User> GetUserAsync(Guid userId);

    Task<string> GenerateEmailConfirmationTokenAsync(User user);

    Task<IdentityResult> ConfirmEmailAsync(User user, string token);

    Task<SignInResult> LoginAsync(LoginDTO model);

    Task LogoutAsync();

    Task<User> GetUserAsync(string email);

    Task<IdentityResult> AddUserAsync(User user, string password);

    Task CheckRoleAsync(string roleName);

    Task AddUserToRoleAsync(User user, string roleName);

    Task<bool> IsUserInRoleAsync(User user, string roleName);
}