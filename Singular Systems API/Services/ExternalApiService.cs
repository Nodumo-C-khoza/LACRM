using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Interfaces;

namespace SingularSystemsAssessment.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ExternalApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = configuration["ExternalApiBaseUrl"] ?? throw new ArgumentNullException("ExternalApiBaseUrl configuration is missing");

            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<IEnumerable<Product>>("products");
                return products ?? Enumerable.Empty<Product>();
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<Product>();
            }
        }

        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            try
            {
                var sales = await _httpClient.GetFromJsonAsync<IEnumerable<Sale>>("sales");
                return sales ?? Enumerable.Empty<Sale>();
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<Sale>();
            }
        }

        public async Task<IEnumerable<Sale>> GetProductSalesAsync(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("ProductId must be greater than zero.", nameof(productId));

            try
            {
                var response = await _httpClient.GetAsync($"product-sales?Id={productId}");
                response.EnsureSuccessStatusCode();

                var sales = await response.Content.ReadFromJsonAsync<List<Sale>>();
                return sales ?? Enumerable.Empty<Sale>();
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<Sale>();
            }
        }
    }
}