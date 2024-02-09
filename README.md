# Gleeman.JwtGenerator

`dotnet` CLI

```bash
$ dotnet add package Gleeman.JwtGenerator --version 7.0.0
```
## HOW TO USE ?

### appsettings.json

```json
 "TokenSetting": {
  "SaveToken": , (default = true) // Optional
  "ValidateIssuer": ,(default = true)  //  Optional
  "ValidateAudience": , (default = true) // Optional
  "ValidateLifetime": , (default = true) // Optional
  "Issuer": "", 
  "Audience": "",
  "SigningKey": "", // Required
  "AccessExpire":  (default = 0),
  "RefreshExpire": (default = 0)
}
```
### Program.cs

```csharp
using Gleeman.JwtGenerator.Configuration;
```

```csharp
builder.Services.AddJwtGenerator(builder.Configuration);
```

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

## EXAMPLE

### appsettings.json

```json
 "TokenSetting": {
  "SaveToken": true, 
  "ValidateIssuer": true,
  "ValidateAudience": true,
  "ValidateLifetime": true,
  "Issuer": "http://localhost:5021",
  "Audience": "http://localhost:5021",
  "SigningKey": "ee98db58bc6847b189f04937b6cb30e3",
  "AccessExpire": 1,
  "RefreshExpire": 2
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

       var token = await _tokenGenerator.GenerateAccessAndRefreshTokenAsync(userParameter, ExpireType.Minute, role: new RoleParameter
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


