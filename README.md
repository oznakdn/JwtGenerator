# Gleeman.JwtGenerator

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
  "Issuer": "This is issuer",
  "Audience": "This is audience",
  "SigningKey": "This is our security key"
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
![Screenshot_1](https://github.com/oznakdn/JwtGenerator/assets/79724084/17814a20-4254-47bd-bb95-5075b8e46035)

