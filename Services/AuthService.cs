using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RestfulApiProject.Data;
using RestfulApiProject.DTOs;
using RestfulApiProject.Models;

namespace RestfulApiProject.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        ApplicationDbContext context,
        ITokenService tokenService,
        ITokenBlacklistService tokenBlacklistService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _tokenService = tokenService;
        _tokenBlacklistService = tokenBlacklistService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = _tokenService.GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);

        return new LoginResponse
        {
            Token = token,
            User = MapToUserDto(user),
            ExpiresAt = expiresAt
        };
    }

    public async Task<UserDto?> RegisterAsync(RegisterRequest request, int? createdBy = null)
    {
        // Check if username or email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

        if (existingUser != null)
            return null;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = request.Role,
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToUserDto(user);
    }

    public async Task<bool> LogoutAsync(string token)
    {
        try
        {
            _tokenBlacklistService.BlacklistToken(token);
            return await Task.FromResult(true);
        }
        catch
        {
            return false;
        }
    }

    public async Task<UserDto?> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user != null ? MapToUserDto(user) : null;
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