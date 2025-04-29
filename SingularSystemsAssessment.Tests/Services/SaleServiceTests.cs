using Microsoft.Extensions.Caching.Memory;
using Moq;
using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Interfaces;
using SingularSystemsAssessment.Services;
using Xunit;

namespace SingularSystemsAssessment.Tests.Services
{
    public class SaleServiceTests
    {
        private readonly Mock<IExternalApiService> _mockExternalApiService;
        private readonly Mock<ISaleRepository> _mockSaleRepository;
        private readonly SaleService _saleService;
        private readonly IMemoryCache _memoryCache;


        public SaleServiceTests()
        {
            _mockExternalApiService = new Mock<IExternalApiService>();
            _mockSaleRepository = new Mock<ISaleRepository>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _saleService = new SaleService(_mockExternalApiService.Object, _mockSaleRepository.Object, _memoryCache);
        }

        [Fact]
        public async Task InitializeSalesDataAsync_AddsSales_WhenEmpty()
        {
            //Arrange
            int productId = 1;
            _mockSaleRepository.Setup(r => r.HasProductSalesAsync(productId)).ReturnsAsync(false);
            var externalSales = new List<Sale> { new Sale { SaleId = 1, ProductId = productId } };
            _mockExternalApiService.Setup(api => api.GetProductSalesAsync(productId)).ReturnsAsync(externalSales);
            _mockSaleRepository.Setup(r => r.AddRangeAsync(externalSales)).Returns(Task.CompletedTask);

            // Act
            await _saleService.InitializeSalesDataAsync(productId);

            // Assert
            _mockSaleRepository.Verify(r => r.AddRangeAsync(externalSales), Times.Once);

        }

        [Fact]
        public async Task InitializeSalesDataAsync_DoesNotAddSales_WhenRepositoryNotEmpty()
        {
            // Arrange
            var existingSales = new List<Sale> { new Sale { SaleId = 1 } };
            _mockSaleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(existingSales);

            // Act
            await _saleService.InitializeSalesDataAsync(1);

            // Assert
            _mockSaleRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Sale>>()), Times.Never);
        }

        [Fact]
        public async Task GetSalesAsync_ThrowsArgumentException_WhenProductIdIsInvalid()
        {
            // Arrange
            int invalidProductId = 0;

            // Act 
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _saleService.GetSalesAsync(invalidProductId));

            //Assert
            Assert.Equal("Invalid product ID (Parameter 'productId')", exception.Message);
        }

        [Fact]
        public async Task GetSalesAsync_ReturnsFilteredSalesAndCount()
        {
            // Arrange
            var sales = new List<Sale> { new Sale { ProductId = 1 } };
            _mockExternalApiService.Setup(api => api.GetProductSalesAsync(1)).ReturnsAsync(sales);

            // Act
            var (resultSales, totalRecords) = await _saleService.GetSalesAsync(1);

            // Assert
            Assert.Single(resultSales);
            Assert.Equal(1, totalRecords);
        }
    }
}
