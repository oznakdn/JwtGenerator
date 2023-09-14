using Gleeman.JwtGenerator.Example.Api.Data.Context;
using Microsoft.EntityFrameworkCore;
using Gleeman.JwtGenerator.Configuration;
using Gleeman.JwtGenerator.Example.Api.Services;
using Gleeman.JwtGenerator.Example.Api.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TestDb"));
builder.Services.AddJwtGenerator(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

Database.AddUserData(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
