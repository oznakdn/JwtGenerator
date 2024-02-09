namespace Gleeman.JwtGenerator;

public class TokensResult
{
    public string AccessToken { get; set; }
    public DateTime AccessExpire { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshExpire { get; set; }

}
