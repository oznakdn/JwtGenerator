namespace Gleeman.JwtGenerator.Generator;

public interface ITokenGenerator
{

    TokenResult GenerateAccessToken(UserParameter userParameters, ExpireType expireType, RoleParameter? role = null, List<RoleParameter>? roles = null);
    Task<TokenResult> GenerateAccessTokenAsync(UserParameter userParameters, ExpireType expireType, RoleParameter? role = null, List<RoleParameter>? roles = null);

    TokensResult GenerateAccessAndRefreshToken(UserParameter userParameters, ExpireType expireType, RoleParameter? role = null, List<RoleParameter>? roles = null);
    Task<TokensResult> GenerateAccessAndRefreshTokenAsync(UserParameter userParameters, ExpireType expireType, RoleParameter? role = null, List<RoleParameter>? roles = null);

    TokenResult GenerateRefreshToken(ExpireType expireType);
    Task<TokenResult> GenerateRefreshTokenAsync(ExpireType expireType);

}
