namespace Gleeman.JwtGenerator.Example.Api.Dtos;

public class LoginResponse
{
    public string AccessToken { get; set; }
    public DateTime AccessExpires { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshExpires { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = null;
}


public class LoginResponseMessage : LoginResponse
{
    public LoginResponseMessage(string message)
    {
        Message = message;
    }
    
}