# Gleeman.JwtGenerator

| Package |  Version | Popularity |
| ------- | ----- | ----- |
| `Gleeman.JwtGenerator` | [![NuGet](https://img.shields.io/nuget/v/Gleeman.JwtGenerator.svg)](https://www.nuget.org/packages/Gleeman.JwtGenerator) | [![Nuget](https://img.shields.io/nuget/dt/Gleeman.JwtGenerator.svg)](https://www.nuget.org/packages/Gleeman.JwtGenerator)

<br>

`dotnet` CLI
```
> dotnet add package Gleeman.JwtGenerator --version 1.0.1
> dotnet add package Gleeman.JwtGenerator --version 2.0.0

```
## HOW TO USE ?


### appsettings.json

```csharp
 "TokenSetting": {
    "SaveToken": true, // true or false
    "ValidateIssuer": false, // true or false
    "ValidateAudience": false, // true or false
    "ValidateLifetime": true, // true or false
    "Issuer": null, // string
    "Audience": null, // string
    "SigningKey": "You should be write here your security key!" // string
  }
```
### Program.cs

```
builder.Services.AddJwtGenerator(builder.Configuration);
```

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

## EXAMPLE

### appsettings.json

```csharp
"TokenSetting": {
    "SaveToken": true,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "Issuer": "http://localhost:5021",
    "Audience": "http://localhost:5021",
    "SigningKey": "ee98db58bc6847b189f04937b6cb30e3"
  }
```
### Program.cs

```
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TestDb"));
builder.Services.AddJwtGenerator(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
```

```csharp
Database.AddUserData(app);
app.UseAuthentication();
app.UseAuthorization();
```



#### Service
```csharp

public interface IUserService
{
    Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
}

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
```
#### Controller
```csharp
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var result = await _userService.LoginAsync(loginRequest);
        if (result.Success)
        {
            return Ok(new { AccessToken = result.AccessToken, AccessExpire = result.AccessExpires, RefreshToken = result.RefreshToken, RefreshExpires = result.RefreshExpires });
        }

        return BadRequest(result.Message);
    }
}
```

![Response](https://github.com/oznakdn/JwtGenerator/assets/79724084/11ee41a9-54b6-4bae-bb9f-94db8f189d61)


