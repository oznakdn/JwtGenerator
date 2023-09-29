namespace Gleeman.JwtGenerator;

public class UserParameters
{
    public UserParameters(string userId, string email = null, string username = null, string dateofBirth = null, string mobilePhone = null)
    {
        UserId = userId;
        Email = email;
        Username = username;
        DateOfBirth = dateofBirth;
        MobilePhone = mobilePhone;
    }
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string DateOfBirth { get; set; }
    public string MobilePhone { get; set; }
}

