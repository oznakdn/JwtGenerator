using Gleeman.JwtGenerator.Helpers;

namespace Gleeman.JwtGenerator.Generator;

public class TokenGenerator : ITokenGenerator
{
    private readonly TokenSetting _setting;
    private List<Claim> claims = new();
    public TokenGenerator(IOptions<TokenSetting> setting)
    {
        _setting = setting.Value;
    }
    public TokenResult GenerateAccessToken(UserParameter userParameters, ExpireType expireType, int ExpireTime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SigningKey!));

        SigningCredentials _signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        DateTime _expires = TokenExpireHelper.SetExpireDate(expireType, ExpireTime);

        ClaimsIdentity claimsIdentity = new();
        claims = AddClaimsToToken(userParameters);
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
        return new TokenResult
        {
            Token = token,
            ExpireDate = _expires
        };
    }


    public TokenResult GenerateAccessToken(UserParameter userParameters, List<RoleParameter> roleParameters, ExpireType expireType, int ExpireTime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SigningKey!));

        SigningCredentials _signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        DateTime _expires = TokenExpireHelper.SetExpireDate(expireType, ExpireTime);

        ClaimsIdentity claimsIdentity = new();
        claims = AddClaimsToToken(userParameters, roleParameters);
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
        return new TokenResult
        {
            Token = token,
            ExpireDate = _expires
        };
    }


    public TokenResult GenerateAccessToken(UserParameter userParameters, RoleParameter roleParameters, ExpireType expireType, int ExpireTime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SigningKey!));

        SigningCredentials _signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        DateTime _expires = TokenExpireHelper.SetExpireDate(expireType, ExpireTime);

        ClaimsIdentity claimsIdentity = new();
        claims = AddClaimsToToken(userParameters, roleParameters);
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
        return new TokenResult
        {
            Token = token,
            ExpireDate = _expires
        };
    }



    public TokenResult GenerateRefreshToken(ExpireType expireType, int ExpireTime)
    {
        DateTime expires = TokenExpireHelper.SetExpireDate(expireType, ExpireTime);
        var randomNumber = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(randomNumber);
        string refreshToken = Convert.ToBase64String(randomNumber);
        return new TokenResult
        {
            Token = refreshToken,
            ExpireDate = expires
        };
    }


    private List<Claim> AddClaimsToToken(UserParameter userParameters)
    {

        claims.Add(new Claim(ClaimTypes.NameIdentifier, userParameters.Id));

        if (!string.IsNullOrEmpty(userParameters.Email))
        {
            claims!.Add(new Claim(ClaimTypes.Email, userParameters.Email!));
        }

        if (!string.IsNullOrEmpty(userParameters.Username))
        {
            claims!.Add(new Claim(ClaimTypes.Name, userParameters.Username!));
        }

        if (userParameters.DateOfBirth != null)
        {
            claims!.Add(new Claim(ClaimTypes.DateOfBirth, userParameters.DateOfBirth.ToString()));
        }

        if (!string.IsNullOrEmpty(userParameters.MobilePhone))
        {
            claims!.Add(new Claim(ClaimTypes.MobilePhone, userParameters.MobilePhone));
        }


        return claims!;
    }

    private List<Claim> AddClaimsToToken(UserParameter userParameters, List<RoleParameter> roles = null)
    {

        claims.Add(new Claim(ClaimTypes.NameIdentifier, userParameters.Id));

        if (!string.IsNullOrEmpty(userParameters.Email))
        {
            claims!.Add(new Claim(ClaimTypes.Email, userParameters.Email!));
        }

        if (!string.IsNullOrEmpty(userParameters.Username))
        {
            claims!.Add(new Claim(ClaimTypes.Name, userParameters.Username!));
        }

        if (userParameters.DateOfBirth != null)
        {
            claims!.Add(new Claim(ClaimTypes.DateOfBirth, userParameters.DateOfBirth.ToString()));
        }

        if (!string.IsNullOrEmpty(userParameters.MobilePhone))
        {
            claims!.Add(new Claim(ClaimTypes.MobilePhone, userParameters.MobilePhone));
        }

        if (roles.Any())
        {
            foreach (var role in roles)
            {
                if (!string.IsNullOrEmpty(role.Role))
                {
                    claims!.Add(new Claim(ClaimTypes.Role, role.Role));
                }
            }
        }

        return claims!;
    }

    private List<Claim> AddClaimsToToken(UserParameter userParameters, RoleParameter role = null)
    {

        claims.Add(new Claim(ClaimTypes.NameIdentifier, userParameters.Id));

        if (!string.IsNullOrEmpty(userParameters.Email))
        {
            claims!.Add(new Claim(ClaimTypes.Email, userParameters.Email!));
        }

        if (!string.IsNullOrEmpty(userParameters.Username))
        {
            claims!.Add(new Claim(ClaimTypes.Name, userParameters.Username!));
        }

        if (userParameters.DateOfBirth != null)
        {
            claims!.Add(new Claim(ClaimTypes.DateOfBirth, userParameters.DateOfBirth.ToString()));
        }

        if (!string.IsNullOrEmpty(userParameters.MobilePhone))
        {
            claims!.Add(new Claim(ClaimTypes.MobilePhone, userParameters.MobilePhone));
        }

        if (role != null)
        {

            if (!string.IsNullOrEmpty(role.Role))
            {
                claims!.Add(new Claim(ClaimTypes.Role, role.Role));
            }

        }

        return claims!;
    }
}
