using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TechStock.Models;

// Modell för produkter
public class Product
{
    public int Id { get; set; }

    [Display(Name = "Artikelnummer")]
    [ValidateNever] // Ignorera validering för att undvika felmeddelande
    public string ArticleNumber { get; set; } = string.Empty;

    [Display(Name = "Produktnamn")]
    [Required(ErrorMessage = "Ange produktnamn")]
    [StringLength(50, ErrorMessage = "Namnet får vara max 50 tecken")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Produktbeskrivning")]
    [Required(ErrorMessage = "Ange produktbeskrivning")]
    [StringLength(500, ErrorMessage = "Beskrivningen får vara max 500 tecken")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Pris")]
    [Required(ErrorMessage = "Ange produktens pris")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Priset måste vara större än 0")]
    public decimal Price { get; set; }

    [Display(Name = "Vikt (g)")]
    [Required(ErrorMessage = "Ange produktens vikt")]
    [Range(0, int.MaxValue, ErrorMessage = "Vikten måste vara större än 0")]
    public int Weight { get; set; }

    [Display(Name = "Lagerantal")]
    [Required(ErrorMessage = "Ange lagerantal")]
    [Range(0, int.MaxValue, ErrorMessage = "Lagerantalet måste vara 0 eller större")]
    public int StockQuantity { get; set; }

    [Display(Name = "Produktbild")]
    public string? ImageName { get; set; }

    [Display(Name = "Produktbild")]
    [NotMapped] // Sparas inte i databasen utan används för att ladda upp filer
    public IFormFile? ImageFile { get; set; }

    // Relation till kategori
    [Display(Name = "Kategori")]
    [Required(ErrorMessage = "Välj produktkategori")]
    public int CategoryId { get; set; } // Främmandenyckel

    [Display(Name = "Kategori")]
    [ValidateNever] // Ignorera validering för att undvika felmeddelande
    public Category Category { get; set; } = null!; // Navigation Property

    // Relation till varumärke
    [Display(Name = "Varumärke")]
    [Required(ErrorMessage = "Välj produktens varumärke")]
    public int BrandId { get; set; }

    [Display(Name = "Varumärke")]
    [ValidateNever] // Ignorera validering för att undvika felmeddelande
    public Brand Brand { get; set; } = null!;

    // Datum för skapande och uppdatering
    [Display(Name = "Skapad")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Senast uppdaterad")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped] // Sparas inte i databasen utan beräknas automatiskt
    public string StockStatus => GetStockStatus();

    // Metod som beräknar status baserat på lagerantal
    private string GetStockStatus()
    {
        if (StockQuantity == 0)
            return "Slut i lager";
        else if (StockQuantity <= 10)
            return "Få kvar";
        else
            return "I lager";
    }
}
