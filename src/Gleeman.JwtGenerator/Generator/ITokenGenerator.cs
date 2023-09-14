namespace Gleeman.JwtGenerator.Generator;

public interface ITokenGenerator
{
    TokenResult GenerateAccessToken(TokenParameter tokenParameter, ExpireType expireType, int ExpireTime);
    TokenResult GenerateRefreshToken(ExpireType expireType, int ExpireTime);
}
