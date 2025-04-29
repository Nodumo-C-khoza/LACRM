using Microsoft.Extensions.Caching.Memory;
using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Interfaces;
using SingularSystemsAssessment.Models;

namespace SingularSystemsAssessment.Services
{
    public class ProductService : IProductService
    {

        private readonly IProductRepository _productRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ISaleService _saleService;
        private readonly IExternalApiService _externalApiService;
        private readonly IMemoryCache _cache;
        private const string ProductsCacheKey = "products_cache_key";


        public ProductService(IProductRepository productRepository, IExternalApiService externalApiService, ISaleRepository saleRepository,ISaleService saleService, IMemoryCache cache)
        {
            _productRepository = productRepository;
            _externalApiService = externalApiService;
            _saleRepository = saleRepository;
            _cache = cache;
            _saleService = saleService;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            if (!_cache.TryGetValue(ProductsCacheKey, out IEnumerable<Product> products))
            {
                products = await _productRepository.GetAllAsync();

                if (products == null || !products.Any())
                {
                    var externalProducts = await _externalApiService.GetProductsAsync();
                    if (externalProducts != null && externalProducts.Any())
                    {
                        await _productRepository.AddRangeAsync(externalProducts);
                        products = externalProducts;
                    }
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(ProductsCacheKey, products, cacheEntryOptions);
            }

            return products;
        }


        public async Task<IEnumerable<ProductSalesSummaryViewModel>> GetSalesSummaryAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var sales = await _saleRepository.GetAllAsync();

            var summary = products.Select(p =>
            {
                var totalSales = sales.Where(s => s.ProductId == p.Id).Sum(s => s.SalePrice * s.SaleQty);

                return new ProductSalesSummaryViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Description,
                    TotalSales = totalSales
                };
            });

            return summary;
        }

        public async Task<IEnumerable<ProductSalesSummaryViewModel>> GetSalesSummaryByProductIdsAsync()
        {
            var products = await _externalApiService.GetProductsAsync();
            var summaries = new List<ProductSalesSummaryViewModel>();

            foreach (var product in products)
            {
                var (sales, _) = await _saleService.GetSalesAsync(productId: product.Id);
                summaries.Add(new ProductSalesSummaryViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Description,
                    TotalQuantity = sales.Sum(s => s.SaleQty),
                    TotalSales = sales.Sum(s => s.SalePrice * s.SaleQty)
                });
            }

            return summaries;
        

    }
    }
}