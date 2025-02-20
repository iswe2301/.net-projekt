namespace TechStock.Models;

// DTO-modell för att returnera data från API
public class ProductDto
{
    public int Id { get; set; }
    public string ArticleNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Weight { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageName { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string StockStatus { get; set; } = string.Empty;
}
