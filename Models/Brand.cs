using System.ComponentModel.DataAnnotations;

namespace TechStock.Models;

// Modell för varumärken
public class Brand
{
    public int Id { get; set; }

    [Display(Name = "Varumärke")]
    [Required(ErrorMessage = "Ange varumärkets namn")]
    [StringLength(50, ErrorMessage = "Namnet får vara max 50 tecken")]
    public string Name { get; set; } = string.Empty;

    // Navigation Property
    public ICollection<Product> Products { get; set; } = new List<Product>(); 
}
