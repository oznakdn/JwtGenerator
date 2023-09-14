using Gleeman.JwtGenerator.Example.Api.Controllers;
using Gleeman.JwtGenerator.Example.Api.Dtos;
using Gleeman.JwtGenerator.Example.Api.Entities;
using Gleeman.JwtGenerator.Example.Api.Services;
using Moq;

namespace Gleeman.JwtGenerator.Tests;

public class ServiceTests
{
    private Mock<IUserService> _moqService;
    private AuthController _authController;
    private User User { get; }
    private Role Role { get; }
    public ServiceTests()
    {
        _moqService = new Mock<IUserService>();
        _authController = new AuthController(_moqService.Object);

        Role = new()
        {
            Id = 1,
            RoleName = "Admin"
        };
        User = new()
        {
            Id = 1,
            UserName = "John_Doe",
            Email = "john.doe@mail.com",
            Password = "password123",
            DateOfBirth = new DateTime(2000, 1, 1),
            PhoneNumber = "5004002010",
            RoleId = 1
        };
    }

    [Fact]
    void Login_When_RequestParameter_Valid_Return_OkResult()
    {
        var expectedResult = new LoginResponse();
        var loginRequest = new LoginRequest("john.doe@mail.com", "password123");
        _moqService.Setup(x => x.LoginAsync(loginRequest)).ReturnsAsync(expectedResult);
        var actualResult = _authController.Login(loginRequest);
        
    }
}
