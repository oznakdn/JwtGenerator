namespace Gleeman.JwtGenerator.Generator;

public interface ITokenGenerator
{

    /// <summary>
    /// Used if the claim has no role. Generates an 'access' token.
    /// </summary>
    /// <returns>TokenResult</returns>
    TokenResult GenerateAccessToken(UserParameters userParameter, ExpireType expireType, int ExpireTime);

    /// <summary>
    /// Used if the claim has more than one role. Generates an 'access' token
    /// <returns>TokenResult</returns>
    TokenResult GenerateAccessToken(UserParameters  userParameter, List<RoleParameters> roles, ExpireType expireType, int ExpireTime);

    /// <summary>
    /// Used if the person has only one role. Generates an 'access' token
    /// </summary>
    /// <returns>TokenResult</returns>
    TokenResult GenerateAccessToken(UserParameters userParameter, RoleParameters role, ExpireType expireType, int ExpireTime);

    /// <summary>
    /// It is used when you want to generate a 'refresh' token.
    /// </summary>
    /// <returns>TokenResult</returns>
    TokenResult GenerateRefreshToken(ExpireType expireType, int ExpireTime);
}
