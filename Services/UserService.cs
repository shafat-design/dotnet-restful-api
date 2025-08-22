using Microsoft.EntityFrameworkCore;
using RestfulApiProject.Data;
using RestfulApiProject.DTOs;
using RestfulApiProject.Models;

namespace RestfulApiProject.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return users.Select(MapToUserDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserRequest request, int updatedBy)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return null;

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            // Check if username already exists for another user
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Id != id);
            if (existingUser != null)
                return null; // Username already taken
            
            user.Username = request.Username;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            // Check if email already exists for another user
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != id);
            if (existingUser != null)
                return null; // Email already taken
            
            user.Email = request.Email;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        if (request.Role.HasValue)
        {
            user.Role = request.Role.Value;
        }

        user.UpdatedBy = updatedBy;
        await _context.SaveChangesAsync();

        return MapToUserDto(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy
        };
    }
}