using Gleeman.JwtGenerator.Example.Api.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gleeman.JwtGenerator.Example.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public ProductsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult>GetProducts()
    {
        var products = await _dbContext.Products.ToListAsync();
        return Ok(products);
    }
}
