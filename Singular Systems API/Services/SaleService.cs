using Microsoft.Extensions.Caching.Memory;
using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Interfaces;

public class SaleService : ISaleService
{
    private readonly IExternalApiService _externalApiService;
    private readonly ISaleRepository _saleRepository;
    private readonly IMemoryCache _cache;
    private const int MaxPageSize = 50;
    private const string SalesCacheKeyPrefix = "sales_product_";

    public SaleService(
        IExternalApiService externalApi,
        ISaleRepository saleRepository,
        IMemoryCache cache) // Add IMemoryCache
    {
        _externalApiService = externalApi;
        _saleRepository = saleRepository;
        _cache = cache;
    }

    private async Task<IEnumerable<Sale>> GetCachedSalesAsync(int productId)
    {
        string cacheKey = $"{SalesCacheKeyPrefix}{productId}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Sale> sales))
        {
            sales = await _externalApiService.GetProductSalesAsync(productId);

            _cache.Set(cacheKey, sales, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        return sales;
    }

    public async Task<(IEnumerable<Sale> Sales, int TotalRecords)> GetSalesAsync(
        int productId, DateTime? saleDate = null)
    {
        if (productId <= 0) throw new ArgumentException("Invalid product ID", nameof(productId));

        var allSales = await GetCachedSalesAsync(productId);

        if (saleDate.HasValue)
        {
            allSales = allSales.Where(s => s.SaleDate.Date == saleDate.Value.Date);
        }

        return (
            Sales: allSales.OrderBy(s => s.SaleDate).Take(MaxPageSize),
            TotalRecords: allSales.Count()
        );
    }

    public async Task InitializeSalesDataAsync(int productId)
    {
        bool hasSales = await _saleRepository.HasProductSalesAsync(productId);
        if (!hasSales)
        {
            var externalSales = await GetCachedSalesAsync(productId);
            if (externalSales.Any())
            {
                await _saleRepository.AddRangeAsync(externalSales);
            }
        }
    }
}
