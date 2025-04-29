using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Services;
using System.Net;
using System.Text.Json;
using Xunit;
namespace SingularSystemsAssessment.Tests.Services
{

    public class ExternalApiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly HttpClient _httpClient;
        private readonly ExternalApiService _apiService;
        private readonly IConfiguration _config;

        public ExternalApiServiceTests()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHandler.Object)
            {
                BaseAddress = new Uri("https://mock-api.com/")
            };

            var inMemoryConfig = new Dictionary<string, string>
        {
            { "ExternalApiBaseUrl", "https://mock-api.com/" }
        };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            _apiService = new ExternalApiService(_httpClient, _config);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsProducts_OnSuccess()
        {
            // Arrange
            var products = new List<Product> { new Product { Id = 1 } };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(products))
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiService.GetProductsAsync();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProductSalesAsync_ThrowsException_ForInvalidProductId()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _apiService.GetProductSalesAsync(0)
            );
        }

        [Fact]
        public async Task GetProductSalesAsync_ReturnsEmpty_OnHttpFailure()
        {
            // Arrange
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            // Act
            var result = await _apiService.GetProductSalesAsync(1);

            // Assert
            Assert.Empty(result);
        }
    }
}
