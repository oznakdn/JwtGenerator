using Gleeman.JwtGenerator.Example.Api.Dtos;

namespace Gleeman.JwtGenerator.Example.Api.Services;

public interface IUserService
{
    Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
}
