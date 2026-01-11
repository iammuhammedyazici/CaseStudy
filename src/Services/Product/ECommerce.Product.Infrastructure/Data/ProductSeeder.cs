using ECommerce.Product.Application.Abstractions.Search;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Infrastructure.Data;

namespace ECommerce.Product.Infrastructure.Data;

public static class ProductSeeder
{
    public static async Task SeedAsync(ProductDbContext context, IElasticsearchService? elasticsearchService = null)
    {
        if (await context.Database.CanConnectAsync() && !context.Products.Any())
        {
            var products = new List<Domain.Entities.Product>
            {
                new() {
                    Name = "Classic T-Shirt",
                    Description = "Comfortable cotton t-shirt for everyday wear",
                    Category = "Clothing",
                    Brand = "BasicWear",
                    ImageUrl = "https://picsum.photos/400/400?random=1",
                    Variants = new List<ProductVariant>
                    {
                        new() { SKU = "TS-BLK-S", Name = "Black - Small", Price = 19.99m, StockQuantity = 50, Color = "Black", Size = "S" },
                        new() { SKU = "TS-BLK-M", Name = "Black - Medium", Price = 19.99m, StockQuantity = 100, Color = "Black", Size = "M" },
                        new() { SKU = "TS-BLK-L", Name = "Black - Large", Price = 19.99m, StockQuantity = 75, Color = "Black", Size = "L" },
                        new() { SKU = "TS-WHT-M", Name = "White - Medium", Price = 19.99m, StockQuantity = 80, Color = "White", Size = "M" },
                        new() { SKU = "TS-WHT-L", Name = "White - Large", Price = 19.99m, StockQuantity = 60, Color = "White", Size = "L" }
                    }
                },
                new() {
                    Name = "Running Shoes",
                    Description = "Lightweight running shoes with excellent cushioning",
                    Category = "Footwear",
                    Brand = "SpeedFit",
                    ImageUrl = "https://picsum.photos/400/400?random=2",
                    Variants = new List<ProductVariant>
                    {
                        new() { SKU = "RS-BLU-40", Name = "Blue - Size 40", Price = 89.99m, StockQuantity = 30, Color = "Blue", Size = "40" },
                        new() { SKU = "RS-BLU-42", Name = "Blue - Size 42", Price = 89.99m, StockQuantity = 40, Color = "Blue", Size = "42" },
                        new() { SKU = "RS-GRY-41", Name = "Gray - Size 41", Price = 89.99m, StockQuantity = 35, Color = "Gray", Size = "41" },
                        new() { SKU = "RS-GRY-43", Name = "Gray - Size 43", Price = 89.99m, StockQuantity = 25, Color = "Gray", Size = "43" }
                    }
                },
                new() {
                    Name = "Wireless Headphones",
                    Description = "Premium noise-cancelling wireless headphones",
                    Category = "Electronics",
                    Brand = "SoundMax",
                    ImageUrl = "https://picsum.photos/400/400?random=3",
                    Variants = new List<ProductVariant>
                    {
                        new() { SKU = "WH-BLK-STD", Name = "Black - Standard", Price = 149.99m, StockQuantity = 50, Color = "Black" },
                        new() { SKU = "WH-SLV-STD", Name = "Silver - Standard", Price = 149.99m, StockQuantity = 40, Color = "Silver" }
                    }
                },
                new() {
                    Name = "Laptop Backpack",
                    Description = "Durable backpack with padded laptop compartment",
                    Category = "Accessories",
                    Brand = "TechPack",
                    ImageUrl = "https://picsum.photos/400/400?random=4",
                    Variants = new List<ProductVariant>
                    {
                        new() { SKU = "BP-BLK-STD", Name = "Black - Standard", Price = 49.99m, StockQuantity = 60, Color = "Black" },
                        new() { SKU = "BP-GRY-STD", Name = "Gray - Standard", Price = 49.99m, StockQuantity = 45, Color = "Gray" },
                        new() { SKU = "BP-NVY-STD", Name = "Navy - Standard", Price = 49.99m, StockQuantity = 30, Color = "Navy" }
                    }
                },
                new() {
                    Name = "Smart Watch",
                    Description = "Fitness tracking smartwatch with heart rate monitor",
                    Category = "Electronics",
                    Brand = "FitTrack",
                    ImageUrl = "https://picsum.photos/400/400?random=5",
                    Variants = new List<ProductVariant>
                    {
                        new() { SKU = "SW-BLK-42MM", Name = "Black - 42mm", Price = 199.99m, StockQuantity = 25, Color = "Black", Size = "42mm" },
                        new() { SKU = "SW-ROSE-38MM", Name = "Rose Gold - 38mm", Price = 199.99m, StockQuantity = 20, Color = "Rose Gold", Size = "38mm" }
                    }
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            if (elasticsearchService != null)
            {
                try
                {
                    await elasticsearchService.IndexProductsAsync(products, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to index products to Elasticsearch: {ex.Message}");
                }
            }
        }
    }
}
