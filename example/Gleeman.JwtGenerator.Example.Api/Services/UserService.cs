using Gleeman.JwtGenerator.Example.Api.Data.Context;
using Gleeman.JwtGenerator.Example.Api.Dtos;
using Gleeman.JwtGenerator.Generator;
using Microsoft.EntityFrameworkCore;

namespace Gleeman.JwtGenerator.Example.Api.Services;

public class UserService : IUserService
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly AppDbContext _dbContext;
    public UserService(ITokenGenerator tokenGenerator, AppDbContext dbContext)
    {
        _tokenGenerator = tokenGenerator;
        _dbContext = dbContext;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _dbContext.Users
            .Where(x => x.Email == loginRequest.Email && x.Password == loginRequest.Password)
            .Include(x => x.Role)
            .SingleOrDefaultAsync();

        if (user == null)
        {
            return new LoginResponseMessage("Email or Password is wrong!") { Success = false };
        }

        var accessToken = _tokenGenerator.GenerateAccessToken(
            new TokenParameter(
            email:user.Email,
            username:user.UserName!,
            role:user.Role.RoleName,
            dateofBirth:user.DateOfBirth.ToString()!,
            mobilePhone:user.PhoneNumber!), 
            ExpireType.Minute, 
            5);

        var refreshToken = _tokenGenerator.GenerateRefreshToken(
            ExpireType.Minute, 
            10);

        user.Token = refreshToken.Token;
        user.TokenExpire = refreshToken.ExpireDate;
        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
        return new LoginResponse
        {
            AccessToken = accessToken.Token,
            AccessExpires = accessToken.ExpireDate,
            RefreshToken = refreshToken.Token,
            RefreshExpires = refreshToken.ExpireDate,
            Success = true
        };
    }
}
