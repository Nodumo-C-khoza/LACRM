
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Interfaces;
using SingularSystemsAssessment.Services;
using Xunit;

namespace SingularSystemsAssessment.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<ISaleRepository> _mockSaleRepo;
        private readonly Mock<ISaleService> _mockSaleService;
        private readonly Mock<IExternalApiService> _mockExternalApiService;
        private readonly IMemoryCache _memoryCache;
        private readonly ProductService _productService;

        private readonly List<Product> _sampleProducts = new()
    {
        new Product { Id = 1, Description = "Apples", SalePrice = 15.33m, Category = "Fruit" },
        new Product { Id = 2, Description = "Oranges", SalePrice = 11.57m, Category = "Fruit" },
        new Product { Id = 3, Description = "Potatoes", SalePrice = 9.99m, Category = "Vegetable" },
        new Product { Id = 4, Description = "Spinach", SalePrice = 12m, Category = "Vegetable",  },
        new Product { Id = 5, Description = "Grapes", SalePrice = 19.99m, Category = "Fruit" },
        // Add more products as needed
    };

        public ProductServiceTests()
        {
            _mockProductRepo = new Mock<IProductRepository>();
            _mockSaleRepo = new Mock<ISaleRepository>();
            _mockSaleService = new Mock<ISaleService>();
            _mockExternalApiService = new Mock<IExternalApiService>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _productService = new ProductService(
                _mockProductRepo.Object,
                _mockExternalApiService.Object,
                _mockSaleRepo.Object,
                _mockSaleService.Object,
                _memoryCache
            );
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsFromCache_WhenPresent()
        {
            // Arrange
            _memoryCache.Set("products_cache_key", _sampleProducts);

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.Equal(_sampleProducts.Count, result.Count());
            Assert.Contains(result, p => p.Description == "Apples");
            _mockProductRepo.Verify(r => r.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetProductsAsync_FetchesFromRepository_WhenCacheIsEmpty()
        {
            // Arrange
            _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(_sampleProducts);

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.Equal(_sampleProducts.Count, result.Count());
            _mockProductRepo.Verify(r => r.GetAllAsync(), Times.Once);
            Assert.True(_memoryCache.TryGetValue("products_cache_key", out IEnumerable<Product> cached));
            Assert.Equal(_sampleProducts.Count, cached.Count());
        }

        [Fact]
        public async Task GetProductsAsync_FetchesFromExternalApi_WhenRepositoryEmpty()
        {
            // Arrange
            _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());
            _mockExternalApiService.Setup(api => api.GetProductsAsync()).ReturnsAsync(_sampleProducts);
            _mockProductRepo.Setup(r => r.AddRangeAsync(_sampleProducts)).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.Equal(_sampleProducts.Count, result.Count());
            _mockProductRepo.Verify(r => r.AddRangeAsync(_sampleProducts), Times.Once);
            Assert.True(_memoryCache.TryGetValue("products_cache_key", out IEnumerable<Product> cached));
            Assert.Equal(_sampleProducts.Count, cached.Count());
        }

        [Fact]
        public async Task GetSalesSummaryAsync_ReturnsCorrectSummary()
        {
            // Arrange
            var sales = new List<Sale>
        {
            new Sale { ProductId = 1, SalePrice = 15.33m, SaleQty = 2 },
            new Sale { ProductId = 2, SalePrice = 11.57m, SaleQty = 3 },
            new Sale { ProductId = 1, SalePrice = 15.33m, SaleQty = 1 }
        };
            _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(_sampleProducts);
            _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(sales);

            // Act
            var summary = (await _productService.GetSalesSummaryAsync()).ToList();

            // Assert
            var applesSummary = summary.First(s => s.ProductId == 1);
            Assert.Equal("Apples", applesSummary.ProductName);
            Assert.Equal(15.33m * 3, applesSummary.TotalSales);

            var orangesSummary = summary.First(s => s.ProductId == 2);
            Assert.Equal("Oranges", orangesSummary.ProductName);
            Assert.Equal(11.57m * 3, orangesSummary.TotalSales);
        }

        [Fact]
        public async Task GetSalesSummaryByProductIdsAsync_ReturnsCorrectSummary()
        {
            // Arrange
            var salesForApples = new List<Sale>
        {
            new Sale { ProductId = 1, SalePrice = 15.33m, SaleQty = 2 },
            new Sale { ProductId = 1, SalePrice = 15.33m, SaleQty = 1 }
        };
            var salesForOranges = new List<Sale>
        {
            new Sale { ProductId = 2, SalePrice = 11.57m, SaleQty = 3 }
        };

            _mockExternalApiService.Setup(api => api.GetProductsAsync()).ReturnsAsync(_sampleProducts);
            _mockSaleService.Setup(api => api.GetSalesAsync(1,null))
                .ReturnsAsync((salesForApples, salesForApples.Count));

            _mockSaleService.Setup(api => api.GetSalesAsync(2, null))
                .ReturnsAsync((salesForOranges, salesForOranges.Count));

            // Act
            var summary = (await _productService.GetSalesSummaryByProductIdsAsync()).ToList();

            // Assert
            var applesSummary = summary.First(s => s.ProductId == 1);
            Assert.Equal("Apples", applesSummary.ProductName);
            Assert.Equal(3, applesSummary.TotalQuantity);
            Assert.Equal(15.33m * 3, applesSummary.TotalSales);

            var orangesSummary = summary.First(s => s.ProductId == 2);
            Assert.Equal("Oranges", orangesSummary.ProductName);
            Assert.Equal(3, orangesSummary.TotalQuantity);
            Assert.Equal(11.57m * 3, orangesSummary.TotalSales);
        }
    }
}
