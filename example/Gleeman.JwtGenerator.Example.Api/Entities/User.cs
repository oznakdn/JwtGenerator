namespace Gleeman.JwtGenerator.Example.Api.Entities;

public class User
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Token { get; set; }
    public DateTime? TokenExpire { get; set; }

    public int? RoleId { get; set; }
    public Role Role { get; set; }
}
