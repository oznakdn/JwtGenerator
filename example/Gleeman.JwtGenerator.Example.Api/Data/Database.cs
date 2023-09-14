using Gleeman.JwtGenerator.Example.Api.Data.Context;
using Gleeman.JwtGenerator.Example.Api.Entities;

namespace Gleeman.JwtGenerator.Example.Api.Data;

public class Database
{
    public static void AddUserData(WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<AppDbContext>();

        db.Roles.Add(new Role
        {
            Id = 1,
            RoleName = "Admin"
        });

        db.Users.Add(new User
        {
            Id = 1,
            UserName = "John_Doe",
            Email = "john.doe@mail.com",
            Password = "password123",
            DateOfBirth = new DateTime(2000,1, 1),
            PhoneNumber = "5004002010",
            RoleId = 1
        });


        db.SaveChanges();
    }
}