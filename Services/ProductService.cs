using TechStock.Data;
using Microsoft.EntityFrameworkCore;

namespace TechStock.Services;

// Serviceklass för att generera unika artikelnummer
public class ProductService
{

    // Databasanslutning
    private readonly ApplicationDbContext _context;

    // Konstruktor
    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Metod för att generera artikelnummer
    public async Task<string> GenerateArticleNumber()
    {
        // Egenskaper för artikelnummer och kontroll om artikelnummer redan finns
        string articleNumber;
        bool exists;

// Loopa igenom databasen
        do
        {
            // Generera artikelnummer med år och unikt nummer (5 tecken) från Guid 
            articleNumber = $"TS-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N").Substring(0, 5)}";
            // Kontrollera om artikelnummer redan finns i databasen
            exists = await _context.Products.AnyAsync(p => p.ArticleNumber == articleNumber);
        } while (exists); // Upprepa så länge artikelnummer redan finns

        return articleNumber; // Returnera unikt artikelnummer
    }
}
