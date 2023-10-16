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



        /******************************** If user has more role ******************************
         
        var roleParameters = new List<RoleParameter>();
        roleParameters = user.Roles.Select(x => new RoleParameters
        {
            Role = x.RoleName
        }).ToList();

        var accessToken = _tokenGenerator.GenerateAccessToken(userParameter, roleParameter,
                          ExpireType.Minute,
                          5);
         *************************************************************************************/



        /******************************** If user has a role ***********************************
          var roleParameter = new RoleParameter();
          roleParameter.Role = user.Role.RoleName;

          var accessToken = _tokenGenerator.GenerateAccessToken(userParameter,roleParameter,
                            ExpireType.Minute,
                            5);
         ***************************************************************************************/



        // If user has no a role
        var accessToken = _tokenGenerator.GenerateAccessToken(userParameter,
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
