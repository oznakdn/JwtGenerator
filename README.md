# Gleeman.JwtGenerator

<h2>HOW TO USE</h2>


<h4>Copy to appsettings.json and configure the parameters</h4>

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
<h4>Add to program.cs</h4>

```
builder.Services.AddJwtGenerator(builder.Configuration);
```

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

### Usage

#### Service
```csharp

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
