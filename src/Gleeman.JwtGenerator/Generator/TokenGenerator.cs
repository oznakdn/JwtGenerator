

namespace Gleeman.JwtGenerator.Generator;

public class TokenGenerator : ITokenGenerator
{
    private readonly TokenSetting _setting;

    private List<Claim> claims = new();
    public TokenGenerator(IOptions<TokenSetting> setting)
    {
        _setting = setting.Value;
    }

    public TokensResult GenerateAccessAndRefreshToken(UserParameter userParameters, ExpireType expireType, RoleParameter? role = null, List<RoleParameter>? roles = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SigningKey!));

        SigningCredentials _signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        DateTime _expires = SetAccessExpireDate(expireType);

        ClaimsIdentity claimsIdentity = new();
        claims = AddClaimsToToken(userParameters, role, roles);
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
        var accessToken = tokenHandler.WriteToken(createToken);
        var refreshToken = GenerateRefreshToken(expireType);
        return new TokensResult
        {
            AccessToken = accessToken,
            AccessExpire = _expires,
            RefreshToken = refreshToken.Token,
            RefreshExpire = refreshToken.ExpireDate
        };
    }

    public async Task<TokensResult> GenerateAccessAndRefreshTokenAsync(UserParameter userParameters, ExpireType expireType, RoleParameter? role = null, List<RoleParameter>? roles = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SigningKey!));

        SigningCredentials _signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        DateTime _expires = SetAccessExpireDate(expireType);

        ClaimsIdentity claimsIdentity = new();
        claims = AddClaimsToToken(userParameters, role, roles);
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
        var accessToken = tokenHandler.WriteToken(createToken);
        var refreshToken = await GenerateRefreshTokenAsync(expireType);
        var result = new TokensResult
        {
            AccessToken = accessToken,
            AccessExpire = _expires,
            RefreshToken = refreshToken.Token,
            RefreshExpire = refreshToken.ExpireDate
        };

        return await Task.FromResult(result);
    }

    public TokenResult GenerateAccessToken(UserParameter userParameters, ExpireType expireType, RoleParameter? role = null, List<RoleParameter>? roles = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SigningKey!));

        SigningCredentials _signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        DateTime _expires = SetAccessExpireDate(expireType);

        ClaimsIdentity claimsIdentity = new();
        claims = AddClaimsToToken(userParameters, role, roles);
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


    public async Task<TokenResult> GenerateAccessTokenAsync(UserParameter userParameters, ExpireType expireType, RoleParameter? role = null, List<RoleParameter>? roles = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SigningKey!));

        SigningCredentials _signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        DateTime _expires = SetAccessExpireDate(expireType);

        ClaimsIdentity claimsIdentity = new();
        claims = AddClaimsToToken(userParameters, role, roles);
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
        var result = new TokenResult
        {
            Token = token,
            ExpireDate = _expires
        };

        return await Task.FromResult(result);

    }

    public TokenResult GenerateRefreshToken(ExpireType expireType)
    {
        DateTime expires = SetRefreshExpireDate(expireType);
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

    public async Task<TokenResult> GenerateRefreshTokenAsync(ExpireType expireType)
    {
        DateTime expires = SetRefreshExpireDate(expireType);
        var randomNumber = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(randomNumber);
        string refreshToken = Convert.ToBase64String(randomNumber);
        var result = new TokenResult
        {
            Token = refreshToken,
            ExpireDate = expires
        };

        return await Task.FromResult(result);
    }

    private List<Claim> AddClaimsToToken(UserParameter? user, RoleParameter? role = null, List<RoleParameter>? roles = null)
    {


        if (!string.IsNullOrEmpty(user.Id))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        }

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims!.Add(new Claim(ClaimTypes.Email, user.Email!));
        }

        if (!string.IsNullOrEmpty(user.Username))
        {
            claims!.Add(new Claim(ClaimTypes.Name, user.Username!));
        }

        if (user.DateOfBirth != null)
        {
            claims!.Add(new Claim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString()));
        }

        if (!string.IsNullOrEmpty(user.MobilePhone))
        {
            claims!.Add(new Claim(ClaimTypes.MobilePhone, user.MobilePhone));
        }

        if (role is not null)
        {
            claims!.Add(new Claim(ClaimTypes.Role, role.Role));
        }


        if (roles is not null && roles.Count > 0)
        {
            foreach (var item in roles!)
            {
                if (!string.IsNullOrEmpty(item.Role))
                {
                    claims!.Add(new Claim(ClaimTypes.Role, item.Role));
                }
            }
        }


        return claims!;
    }

    private DateTime SetAccessExpireDate(ExpireType expireType)
    {
        DateTime expires = expireType switch
        {
            ExpireType.Second => DateTime.UtcNow.AddSeconds(_setting.AccessExpire),
            ExpireType.Minute => DateTime.UtcNow.AddMinutes(_setting.AccessExpire),
            ExpireType.Hour => DateTime.UtcNow.AddHours(_setting.AccessExpire),
            ExpireType.Day => DateTime.UtcNow.AddDays(_setting.AccessExpire),
            ExpireType.Month => DateTime.UtcNow.AddMonths(_setting.AccessExpire),
            _ => throw new Exception("Expire type not found!")
        };

        return expires;
    }

    private DateTime SetRefreshExpireDate(ExpireType expireType)
    {
        DateTime expires = expireType switch
        {
            ExpireType.Second => DateTime.UtcNow.AddSeconds(_setting.RefreshExpire),
            ExpireType.Minute => DateTime.UtcNow.AddMinutes(_setting.RefreshExpire),
            ExpireType.Hour => DateTime.UtcNow.AddHours(_setting.RefreshExpire),
            ExpireType.Day => DateTime.UtcNow.AddDays(_setting.RefreshExpire),
            ExpireType.Month => DateTime.UtcNow.AddMonths(_setting.RefreshExpire),
            _ => throw new Exception("Expire type not found!")
        };

        return expires;
    }

}
