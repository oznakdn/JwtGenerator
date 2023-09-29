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

        var userParameters = new UserParameters(
            userId: user.Id.ToString(),
            email: user.Email,
            username: user.UserName ?? null,
            dateofBirth: user.DateOfBirth.ToString(),
            mobilePhone: user.PhoneNumber);


        /******************************** If user has more role ******************************
         
        var roleParameters = new List<RoleParameters>();
        roleParameters = user.Roles.Select(x => new RoleParameters
        {
            Role = x.RoleName
        }).ToList();

        var accessToken = _tokenGenerator.GenerateAccessToken(userParameters, roleParameters,
                          ExpireType.Minute,
                          5);
         *************************************************************************************/



        /******************************** If user has a role ***********************************
          var roleParameter = new RoleParameters();
          roleParameter.Role = user.Role.RoleName;

          var accessToken = _tokenGenerator.GenerateAccessToken(userParameters,roleParameter,
                            ExpireType.Minute,
                            5);
         ***************************************************************************************/



        // If user has no a role
        var accessToken = _tokenGenerator.GenerateAccessToken(userParameters,
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
