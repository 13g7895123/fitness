namespace FitnessTracker.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string lineUserId, string displayName);
    (Guid UserId, string LineUserId, string DisplayName) ValidateToken(string token);
}
