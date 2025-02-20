using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechStock.Data;
using TechStock.Models;
using TechStock.Services; // Importera service för produkter
using X.PagedList;
using X.PagedList.Extensions; // Importera X.PagedList.Extensions för att använda paginering


namespace TechStock.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // Hosting Environment för att ladda upp filer
        private readonly string wwwRootPath; // Sökväg till wwwroot-mappen
        private readonly ProductService _productService; // Service för produkter

        public ProductController(ApplicationDbContext context, ProductService productService, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _productService = productService;
            _hostEnvironment = hostEnvironment;
            wwwRootPath = hostEnvironment.WebRootPath;
        }

        // GET: Product
        public IActionResult Index(string searchString, int? page, string sortOrder, int? categoryId, int? brandId, string? stockStatus)
        {
            int pageSize = 10; // Antal produkter per sida
            int pageNumber = page ?? 1; // Aktuell sida, default är 1

            // Lagra kategorier och varumärken som har produkter i en lista
            ViewData["Categories"] = _context.Categories
                .Where(c => _context.Products.Any(p => p.CategoryId == c.Id))
                .ToList();

            ViewData["Brands"] = _context.Brands
                .Where(b => _context.Products.Any(p => p.BrandId == b.Id))
                .ToList();

            // Lagra aktuella filtrerings- och sorteringsvärden
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentBrand = brandId;
            ViewBag.CurrentStockStatus = stockStatus;
            // Hämta produkter inklusive kategori och varumärke
            var products = _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .AsQueryable(); // För att kunna filtrera på söksträng

            // Filtrera produkter baserat på söksträng, kategori, varumärke och lagerstatus
            products = FilterProducts(products, searchString, categoryId, brandId, stockStatus);

            // Lagra totalt antal produkter efter filtrering
            ViewBag.TotalProducts = products.Count();

            // Sortera produkter baserat på valt sorteringssätt
            var productList = SortProducts(products, sortOrder);

            // Paginera produkter
            var pagedProducts = PaginateProducts(productList, pageNumber, pageSize);

            // Returnera vyn
            return View(pagedProducts);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Weight,StockQuantity,ImageFile,CategoryId,BrandId")] Product product)
        {

            // Generera unikt artikelnummer
            product.ArticleNumber = await _productService.GenerateArticleNumber();

            if (string.IsNullOrEmpty(product.ArticleNumber))
            {
                ModelState.AddModelError("ArticleNumber", "Artikelnummer kunde inte genereras");
            }

            if (ModelState.IsValid)
            {
                // Kontrollera om en bild har laddats upp
                if (product.ImageFile != null)
                {
                    // Hämta filnamn och filändelse
                    string fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                    string extension = Path.GetExtension(product.ImageFile.FileName);

                    // Skapa ett unikt filnamn baserat på filnamn och aktuellt datum    
                    product.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("_yyyy-MM-dd-HHmmssfff") + extension;

                    // Spara filen i wwwroot/images-mappen
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    // Spara i filsystemet
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                }

                // Sätter CreatedAt och UpdatedAt till aktuellt datum
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Weight,StockQuantity,ImageName,CategoryId,BrandId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Hämta befintlig produkt från databasen
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                    // Kontrollera om produkten inte finns
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Behåll det befintliga artikelnumret, skapandedatum samt uppdatera uppdateringsdatum
                    product.ArticleNumber = existingProduct.ArticleNumber;
                    product.CreatedAt = existingProduct.CreatedAt;
                    product.UpdatedAt = DateTime.Now;

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // Metod för att paginera produkter
        private IPagedList<Product> PaginateProducts(IEnumerable<Product> products, int pageNumber, int pageSize)
        {
            return products.ToPagedList(pageNumber, pageSize); // Returnera paginerad lista
        }

        // Metod för att sortera produkter baserat på valt sorteringssätt
        private IEnumerable<Product> SortProducts(IQueryable<Product> products, string sortOrder)
        {
            // Konvertera produkter till en lista för att kunna sortera
            var productsList = products.ToList();

            // Sorteringsalternativ (om aktuell sortering är stigande, ändra till fallande, annars till default)
            ViewBag.StockStatusSort = sortOrder == "stockstatus_asc" ? "stockstatus_desc" : sortOrder == "stockstatus_desc" ? "" : "stockstatus_asc";
            ViewBag.ArticleNumberSort = sortOrder == "articlenumber_asc" ? "articlenumber_desc" : sortOrder == "articlenumber_desc" ? "" : "articlenumber_asc";
            ViewBag.NameSort = sortOrder == "name_asc" ? "name_desc" : sortOrder == "name_desc" ? "" : "name_asc";
            ViewBag.PriceSort = sortOrder == "price_asc" ? "price_desc" : sortOrder == "price_desc" ? "" : "price_asc";
            ViewBag.StockQuantitySort = sortOrder == "stockquantity_asc" ? "stockquantity_desc" : sortOrder == "stockquantity_desc" ? "" : "stockquantity_asc";
            ViewBag.CategorySort = sortOrder == "category_asc" ? "category_desc" : sortOrder == "category_desc" ? "" : "category_asc";
            ViewBag.BrandSort = sortOrder == "brand_asc" ? "brand_desc" : sortOrder == "brand_desc" ? "" : "brand_asc";

            // Sortera listan baserat på valt sorteringssätt eller skapandedatum (default)
            switch (sortOrder)
            {
                case "stockstatus_desc":
                    return productsList.OrderByDescending(p => p.StockStatus);
                case "stockstatus_asc":
                    return productsList.OrderBy(p => p.StockStatus);
                case "articlenumber_asc":
                    return productsList.OrderBy(p => p.ArticleNumber);
                case "articlenumber_desc":
                    return productsList.OrderByDescending(p => p.ArticleNumber);
                case "name_asc":
                    return productsList.OrderBy(p => p.Name);
                case "name_desc":
                    return productsList.OrderByDescending(p => p.Name);
                case "price_asc":
                    return productsList.OrderBy(p => p.Price);
                case "price_desc":
                    return productsList.OrderByDescending(p => p.Price);
                case "stockquantity_asc":
                    return productsList.OrderBy(p => p.StockQuantity);
                case "stockquantity_desc":
                    return productsList.OrderByDescending(p => p.StockQuantity);
                case "category_asc":
                    return productsList.OrderBy(p => p.Category.Name);
                case "category_desc":
                    return productsList.OrderByDescending(p => p.Category.Name);
                case "brand_asc":
                    return productsList.OrderBy(p => p.Brand.Name);
                case "brand_desc":
                    return productsList.OrderByDescending(p => p.Brand.Name);
                default:
                    return productsList.OrderByDescending(p => p.CreatedAt);
            }
        }

        // Metod för att filtrera produkter baserat på söksträng
        private IQueryable<Product> FilterProducts(IQueryable<Product> products, string searchString, int? categoryId, int? brandId, string? stockStatus)
        {

            // Kontrollera om söksträngen inte är tom
            if (!string.IsNullOrEmpty(searchString))
            {
                // Konvertera söksträngen till gemener
                searchString = searchString.ToLower();
                // Filtrera produkter baserat på söksträngen (namn eller artikelnummer)
                products = products.Where(p => p.Name.ToLower().Contains(searchString) || p.ArticleNumber.ToLower().Contains(searchString));
            }

            // Kontrollera om kategori har valts och filtrera produkter baserat på kategori
            if (categoryId != null)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            // Kontrollera om varumärke har valts och filtrera produkter baserat på varumärke
            if (brandId != null)
            {
                products = products.Where(p => p.BrandId == brandId);
            }

            // Konvertera produkter till en lista för att kunna filtrera på lagerstatus
            var productList = products.AsEnumerable();

            // Kontrollera om lagerstatus har valts och filtrera produkter baserat på lagerstatus
            if (!string.IsNullOrEmpty(stockStatus))
            {
                productList = stockStatus switch
                {
                    "inStock" => productList.Where(p => p.StockQuantity > 10),
                    "lowStock" => productList.Where(p => p.StockQuantity > 0 && p.StockQuantity <= 10),
                    "outOfStock" => productList.Where(p => p.StockQuantity == 0),
                    _ => productList  // Returnera alla produkter om ingen lagerstatus har valts
                };
            }

            // Konvertera produkter till en IQueryable för att kunna returnera 
            return productList.AsQueryable();
        }
    }
}