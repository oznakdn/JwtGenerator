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

        var userParameter = new UserParameter
        {
            Id = user.Id.ToString(),
            Email= user.Email
        };


        var token =await _tokenGenerator.GenerateAccessAndRefreshTokenAsync(userParameter, ExpireType.Minute, role: new RoleParameter
        {
            Role = user.Role.RoleName
        });


        user.Token = token.RefreshToken;
        user.TokenExpire = token.RefreshExpire;
        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = token.AccessToken,
            AccessExpires = token.AccessExpire,
            RefreshToken = token.RefreshToken,
            RefreshExpires = token.RefreshExpire,
            Success = true
        };

    }
}
