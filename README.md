# Gleeman.JwtGenerator

`dotnet` CLI

```bash
$ dotnet add package Gleeman.JwtGenerator --version 2.0.1
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

       var userParameter = new UserParameter
       {
         Id = user.Id.ToString(),
         Email= user.Email
       };


        /******************************** If user has more role ******************************
         
        var roleParameters = new List<RoleParameter>();
        roleParameters = user.Roles.Select(x => new RoleParameter
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


