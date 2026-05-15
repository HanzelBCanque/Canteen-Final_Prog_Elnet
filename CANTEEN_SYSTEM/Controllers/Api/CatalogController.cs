using CANTEEN_SYSTEM.Contracts;
using CANTEEN_SYSTEM.Data;
using CANTEEN_SYSTEM.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CANTEEN_SYSTEM.Controllers.Api;

[ApiController]
[Route("api/catalog")]
public class CatalogController(CanteenDbContext db) : ControllerBase
{
    [HttpGet("categories")]
    public ActionResult<IReadOnlyList<CategoryDto>> GetCategories()
    {
        var categories = new[]
        {
            new CategoryDto(1, "All"),
            new CategoryDto(2, "Meals"),
            new CategoryDto(3, "Drinks"),
            new CategoryDto(4, "Snacks")
        };

        return Ok(categories);
    }

    [HttpGet("products")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts()
    {
        var products = await db.Products
            .OrderBy(product => product.Id)
            .ToListAsync();

        return Ok(products.Select(product => product.ToDto()).ToList());
    }
}
