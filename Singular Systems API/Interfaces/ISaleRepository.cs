using SingularSystemsAssessment.Entities;

namespace SingularSystemsAssessment.Interfaces
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<IEnumerable<Sale>> GetByProductAsync(
        int productId,
        int page,
        int pageSize);
        Task<bool> HasProductSalesAsync(int productId);
        Task<int> CountByProductAsync(int productId);
        Task<IEnumerable<Sale>> GetFilteredAsync(int? productId, DateTime? startDate, DateTime? endDate, int page, int pageSize);
        Task<int> CountFilteredAsync(int? productId, DateTime? startDate, DateTime? endDate);
        Task AddRangeAsync(IEnumerable<Sale> sales);
        Task<IEnumerable<Sale>> GetSalesByProductIdAsync(int productId);

    }
}
