using System.ComponentModel.DataAnnotations;

namespace TechStock.Models;

// Modell för kategorier
public class Category
{
    public int Id { get; set; }

    [Display(Name = "Kategorinamn")]
    [Required(ErrorMessage = "Ange kategorins namn")]
    [StringLength(50, ErrorMessage = "Namnet får vara max 50 tecken")]
    public string Name { get; set; } = string.Empty;

    // Navigation Property som kopplar samman produkter till kategori
    [Display(Name = "Produkter")]
    public ICollection<Product> Products { get; set; } = new List<Product>(); // Tom lista som default om inga produkter finns till en kategori
}
