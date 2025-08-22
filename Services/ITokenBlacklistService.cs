namespace RestfulApiProject.Services;

public interface ITokenBlacklistService
{
    void BlacklistToken(string token);
    bool IsTokenBlacklisted(string token);
    void CleanupExpiredTokens();
}