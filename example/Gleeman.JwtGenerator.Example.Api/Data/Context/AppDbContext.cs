using Gleeman.JwtGenerator.Example.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gleeman.JwtGenerator.Example.Api.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Product> Products { get; set; }

}
