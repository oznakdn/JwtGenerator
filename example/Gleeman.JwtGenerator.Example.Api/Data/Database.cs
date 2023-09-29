using Gleeman.JwtGenerator.Example.Api.Data.Context;
using Gleeman.JwtGenerator.Example.Api.Entities;

namespace Gleeman.JwtGenerator.Example.Api.Data;

public class Database
{
    public static void AddUserData(WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<AppDbContext>();



        //db.Users.Add(new User
        //{
        //    Id = 1,
        //    UserName = "John_Doe",
        //    Email = "john.doe@mail.com",
        //    Password = "password123",
        //    DateOfBirth = new DateTime(2000, 1, 1),
        //    PhoneNumber = "5004002010",
        //    Roles = new List<Role>
        //    {
        //        new Role
        //        {
        //            Id = 1,
        //            RoleName = "Admin"
        //        },
        //        new Role
        //        {
        //            Id = 2,
        //            RoleName = "Manager"
        //        }
        //    }
        //});

        db.Roles.Add(new Role
        {
            Id = 1,
            RoleName = "Admin"
        });
        db.Roles.Add(new Role
        {
            Id = 2,
            RoleName = "Manager"
        });


        db.Users.Add(new User
        {
            Id = 1,
            UserName = "John_Doe",
            Email = "john.doe@mail.com",
            Password = "password123",
            DateOfBirth = new DateTime(2000, 1, 1),
            PhoneNumber = "5004002010",
            RoleId = 2,
        });


        db.Products.AddRange(
        new Product
        {
            Id = 1,
            Name = "Product1",
            Price = 100,
            Quantity = 1000
        },
         new Product
         {
             Id = 2,
             Name = "Product2",
             Price = 200,
             Quantity = 2000
         },
          new Product
          {
              Id = 3,
              Name = "Product3",
              Price = 300,
              Quantity = 3000
          }
        );


        db.SaveChanges();
    }
}