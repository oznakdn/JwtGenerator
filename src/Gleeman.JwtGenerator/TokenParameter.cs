namespace Gleeman.JwtGenerator;

public class TokenParameter
{
    public TokenParameter(string email = null, string username = null, string role = null, string dateofBirth = null, string mobilePhone = null)
    {
        Email = email;
        Username = username;
        Role = role;
        DateOfBirth = dateofBirth;
        MobilePhone = mobilePhone;
    }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string DateOfBirth { get; set; }
    public string MobilePhone { get; set; }
}
