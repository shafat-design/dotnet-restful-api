using RestfulApiProject.Models;

namespace RestfulApiProject.Services;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    bool ValidateToken(string token);
    int? GetUserIdFromToken(string token);
}