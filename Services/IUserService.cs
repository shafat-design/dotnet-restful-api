using RestfulApiProject.DTOs;

namespace RestfulApiProject.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserRequest request, int updatedBy);
    Task<bool> DeleteUserAsync(int id);
}