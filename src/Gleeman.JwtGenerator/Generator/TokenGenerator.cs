using System.Security.Cryptography;

namespace Gleeman.JwtGenerator.Generator;

public class TokenGenerator : ITokenGenerator
{
    private readonly TokenSetting _setting;
    public TokenGenerator(IOptions<TokenSetting> setting)
    {
        _setting = setting.Value;
    }
    public TokenResult GenerateAccessToken(TokenParameter tokenParameter, ExpireType expireType, int ExpireTime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SigningKey!));

        SigningCredentials _signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        DateTime _expires = SetExpireDate(expireType, ExpireTime);

        ClaimsIdentity claimsIdentity = new();
        var claims = AddClaimsToToken(tokenParameter);
        claimsIdentity!.AddClaims(claims);

        SecurityTokenDescriptor securityTokenDescriptor = new()
        {
            Issuer = _setting.Issuer,
            Audience = _setting.Audience,
            SigningCredentials = _signingCredentials,
            Expires = _expires,
            Subject = claimsIdentity
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var createToken = tokenHandler.CreateToken(securityTokenDescriptor);
        var token = tokenHandler.WriteToken(createToken);
        return new TokenResult(token, _expires);
    }
    public TokenResult GenerateRefreshToken(ExpireType expireType, int ExpireTime)
    {
        DateTime expires = SetExpireDate(expireType, ExpireTime);
        var randomNumber = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(randomNumber);
        string refreshToken = Convert.ToBase64String(randomNumber);
        return new TokenResult(refreshToken, expires);
    }

    DateTime SetExpireDate(ExpireType expireType, int ExpireTime)
    {
        DateTime expires = expireType switch
        {
            ExpireType.Minute => DateTime.Now.AddMinutes(ExpireTime),
            ExpireType.Hour => DateTime.Now.AddHours(ExpireTime),
            ExpireType.Day => DateTime.Now.AddDays(ExpireTime),
            ExpireType.Month => DateTime.Now.AddMonths(ExpireTime),
            _ => throw new Exception("Expire type not found!")
        };

        return expires;
    }

    List<Claim> AddClaimsToToken(TokenParameter tokenParameter)
    {
        List<Claim> claims = new();

        if (!string.IsNullOrEmpty(tokenParameter.Email))
        {
            claims!.Add(new Claim(ClaimTypes.Email, tokenParameter.Email!));
        }

        if (!string.IsNullOrEmpty(tokenParameter.Username))
        {
            claims!.Add(new Claim(ClaimTypes.Name, tokenParameter.Username!));
        }

        if (!string.IsNullOrEmpty(tokenParameter.Role))
        {
            claims!.Add(new Claim(ClaimTypes.Role, tokenParameter.Role!));
        }

        if (tokenParameter.DateOfBirth != null)
        {
            claims!.Add(new Claim(ClaimTypes.DateOfBirth, tokenParameter.DateOfBirth.ToString()));
        }

        if (!string.IsNullOrEmpty(tokenParameter.MobilePhone))
        {
            claims!.Add(new Claim(ClaimTypes.MobilePhone, tokenParameter.MobilePhone));
        }

        return claims!;
    }


}
