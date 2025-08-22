using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;

namespace RestfulApiProject.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

    public void BlacklistToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expirationTime = jwtToken.ValidTo;
            
            _blacklistedTokens.TryAdd(token, expirationTime);
        }
        catch
        {
            // If we can't parse the token, blacklist it with a future date
            _blacklistedTokens.TryAdd(token, DateTime.UtcNow.AddDays(1));
        }
    }

    public bool IsTokenBlacklisted(string token)
    {
        return _blacklistedTokens.ContainsKey(token);
    }

    public void CleanupExpiredTokens()
    {
        var currentTime = DateTime.UtcNow;
        var expiredTokens = _blacklistedTokens
            .Where(kvp => kvp.Value < currentTime)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var expiredToken in expiredTokens)
        {
            _blacklistedTokens.TryRemove(expiredToken, out _);
        }
    }
}