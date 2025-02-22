using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStock.Data; // Importera data
using TechStock.Models; // Importera modell

namespace TechStock.Controllers
{
    [Route("api/produkter")]
    [ApiController]
    // ApiController för att hantera API-anrop, ärver från ControllerBase
    public class ProductApiController : ControllerBase
    {
        // Databasanslutning
        private readonly ApplicationDbContext _context;

        // Konstruktor
        public ProductApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET-metod för att hämta data från databasen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            // Hämta produkter med kategori och varumärke och skapa en lista av ProductDto
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ArticleNumber = p.ArticleNumber,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Weight = p.Weight,
                    StockQuantity = p.StockQuantity,
                    ImageName = p.ImageName,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    BrandId = p.BrandId,
                    BrandName = p.Brand.Name,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    // Skapa en sträng för lagerstatus beroende på lagerantalet
                    StockStatus = p.StockQuantity == 0 ? "Slut i lager"
                        : p.StockQuantity <= 10 ? "Få kvar"
                        : "I lager"
                })
                .ToListAsync();

            return Ok(products); // Returnera produkterna
        }
    }
}
