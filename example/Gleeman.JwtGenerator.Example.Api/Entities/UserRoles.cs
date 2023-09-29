namespace Gleeman.JwtGenerator.Example.Api.Entities;

public class UserRoles
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int UserId { get; set; }

    public User User { get; set; }
    public Role Role { get; set; }
}
