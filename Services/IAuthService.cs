using RestfulApiProject.DTOs;
using RestfulApiProject.Models;

namespace RestfulApiProject.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<UserDto?> RegisterAsync(RegisterRequest request, int? createdBy = null);
    Task<bool> LogoutAsync(string token);
    Task<UserDto?> GetProfileAsync(int userId);
}